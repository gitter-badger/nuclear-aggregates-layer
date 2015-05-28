using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider
{
    public sealed class OperationsQueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IDictionary<StrictOperationIdentity, HashSet<long>> _entityTypeToIds;
        private readonly IQuery _query;
        private readonly QueryRuleContainer<TEntity> _container;
        private readonly IOldOperationContextParser _oldOperationContextParser;

        public OperationsQueryBuilder(IQuery query,
                                      IEnumerable<PerformedBusinessOperation> operations,
                                      QueryRuleContainer<TEntity> container,
                                      IOldOperationContextParser oldOperationContextParser)
        {
            var performedBusinessOperations = operations as PerformedBusinessOperation[] ?? operations.ToArray();

            _query = query;
            _container = container;
            _oldOperationContextParser = oldOperationContextParser;
            _entityTypeToIds = GetEntityTypeToIds(performedBusinessOperations);
        }

        public IQueryable<TEntity> Create(params FindSpecification<TEntity>[] filterSpecifications)
        {
            var curriedRules = from mapping in _entityTypeToIds
                               join expressionEntry in _container.SelectExpressionsByEntity on mapping.Key equals expressionEntry.Key
                               select new Func<IQuery, IQueryable<TEntity>>(finder => expressionEntry.Value(finder, mapping.Value));
            
            if (!curriedRules.Any())
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }

            var baseQ = curriedRules
                .Aggregate<Func<IQuery, IQueryable<TEntity>>, IQueryable<TEntity>>(
                        null,
                        (current, rule) => current == null ? rule(_query) : current.Concat(rule(_query)))
                .Distinct();

            return filterSpecifications.Aggregate(baseQ, (current, filter) => current.Where(filter));
        }

        private Dictionary<StrictOperationIdentity, HashSet<long>> GetEntityTypeToIds(IEnumerable<PerformedBusinessOperation> performedBusinessOperations)
        {
            var operationsToIds = new Dictionary<StrictOperationIdentity, HashSet<long>>();

            var groupedIds = performedBusinessOperations
                .Select(operation => _oldOperationContextParser.GetGroupedIdsFromContext(operation.Context,
                                                                                      operation.Operation,
                                                                                      operation.Descriptor))
                .SelectMany(x => x)
                .ToArray();

            foreach (var @group in groupedIds)
            {
                HashSet<long> ids;
                if (operationsToIds.TryGetValue(@group.Key, out ids))
                {
                    foreach (var id in @group.Value)
                    {
                        ids.Add(id);
                    }
                }
                else
                {
                    operationsToIds[@group.Key] = new HashSet<long>(@group.Value);
                }
            }

            return operationsToIds;
        }
    }
}
