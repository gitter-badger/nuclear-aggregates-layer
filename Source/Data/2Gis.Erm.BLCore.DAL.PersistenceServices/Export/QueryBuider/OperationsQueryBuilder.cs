using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider
{
    public sealed class OperationsQueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IDictionary<StrictOperationIdentity, HashSet<long>> _entityNameToIds;
        private readonly IFinder _finder;
        private readonly QueryRuleContainer<TEntity> _container;
        private readonly IOperationContextParser _operationContextParser;

        public OperationsQueryBuilder(IFinder finder,
                                      IEnumerable<PerformedBusinessOperation> operations,
                                      QueryRuleContainer<TEntity> container,
                                      IOperationContextParser operationContextParser)
        {
            var performedBusinessOperations = operations as PerformedBusinessOperation[] ?? operations.ToArray();

            _finder = finder;
            _container = container;
            _operationContextParser = operationContextParser;
            _entityNameToIds = GetEntityNameToIds(performedBusinessOperations);
        }

        public IQueryable<TEntity> Create(params IFindSpecification<TEntity>[] filterSpecifications)
        {
            var curriedRules = from mapping in _entityNameToIds
                               join expressionEntry in _container.SelectExpressionsByEntity on mapping.Key equals expressionEntry.Key
                               select new Func<IFinder, IQueryable<TEntity>>(finder => expressionEntry.Value(finder, mapping.Value));
            
            if (!curriedRules.Any())
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }

            var baseQ = curriedRules
                .Aggregate<Func<IFinder, IQueryable<TEntity>>, IQueryable<TEntity>>(
                        null,
                        (current, rule) => current == null ? rule(_finder) : current.Concat(rule(_finder)))
                .Distinct();

            return filterSpecifications.Aggregate(baseQ, (current, filter) => current.Where(filter));
        }

        private Dictionary<StrictOperationIdentity, HashSet<long>> GetEntityNameToIds(IEnumerable<PerformedBusinessOperation> performedBusinessOperations)
        {
            var operationsToIds = new Dictionary<StrictOperationIdentity, HashSet<long>>();

            var groupedIds = performedBusinessOperations
                .Select(operation => _operationContextParser.GetGroupedIdsFromContext(operation.Context,
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
