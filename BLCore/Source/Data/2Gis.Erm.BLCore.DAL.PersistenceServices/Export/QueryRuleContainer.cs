using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public sealed class QueryRuleContainer<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IEnumerable<EntityOperationMapping<TEntity>> _operationMappings;

        private QueryRuleContainer(IEnumerable<EntityOperationMapping<TEntity>> operationMappings)
        {
            _operationMappings = operationMappings;
        }

        public IDictionary<StrictOperationIdentity, Func<IFinder, IEnumerable<long>, IQueryable<TEntity>>> SelectExpressionsByEntity
        {
            get { return _operationMappings.ToDictionary(x => x.OperationIdentity, x => x.SelectExpression); }
        }

        public IEnumerable<StrictOperationIdentity> OperationIdentities
        {
            get { return _operationMappings.Select(x => x.OperationIdentity).ToArray(); }
        }

        public static QueryRuleContainer<TEntity> Create(params Func<IEnumerable<EntityOperationMapping<TEntity>>>[] configureActions)
        {
            var operationMappings = configureActions.SelectMany(action => action());
            return new QueryRuleContainer<TEntity>(operationMappings);
        }
    }
}
