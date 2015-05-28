using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    [DebuggerDisplay("{OperationIdentity.OperationIdentity.Description} для {string.Join(\", \", OperationIdentity.Entities)}")]
    public sealed class EntityOperationMapping<TEntity> where TEntity : class, IEntity, IEntityKey
    {
        private readonly StrictOperationIdentity _operationIdentity;
        private readonly Func<IQuery, IEnumerable<long>, IQueryable<TEntity>> _selectExpression;

        private EntityOperationMapping(StrictOperationIdentity operationIdentity, Func<IQuery, IEnumerable<long>, IQueryable<TEntity>> selectExpression)
        {
            _operationIdentity = operationIdentity;
            _selectExpression = selectExpression;
        }

        public StrictOperationIdentity OperationIdentity
        {
            get { return _operationIdentity; }
        }

        public Func<IQuery, IEnumerable<long>, IQueryable<TEntity>> SelectExpression
        {
            get { return _selectExpression; }
        }

        public static ForEntityMapping<TEntity> ForEntity(IEntityType entityType)
        {
            return new ForEntityMapping<TEntity>(entityType);
        }

        public class ForEntityMapping<TEntity> where TEntity : class, IEntity, IEntityKey
        {
            private readonly IEntityType _entityType;
            private readonly List<StrictOperationIdentity> _operationIdentities = new List<StrictOperationIdentity>();

            public ForEntityMapping(IEntityType entityType)
            {
                _entityType = entityType;
            }

            public ForEntityMapping<TEntity> NonCoupledOperation<TOperationIdentity>()
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
            {
                _operationIdentities.Add(new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(_entityType)));
                return this;
            }

            public ForEntityMapping<TEntity> Operation<TOperationIdentity>()
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            {
                var operationIdentity = new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(_entityType));
                _operationIdentities.Add(operationIdentity);
                return this;
            }

            public ForEntityMapping<TEntity> Operation<TOperationIdentity>(IEntityType entityType1, IEntityType entityType2)
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            {
                var operationIdentity = new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(entityType1, entityType2));
                _operationIdentities.Add(operationIdentity);
                return this;
            }

            public IEnumerable<EntityOperationMapping<TEntity>> Use(Func<IQuery, IEnumerable<long>, IQueryable<TEntity>> selectExpression)
            {
                return _operationIdentities.Select(operationIdentity => new EntityOperationMapping<TEntity>(operationIdentity, selectExpression)).ToArray();
            }
        }
    }
}
