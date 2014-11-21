using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    [DebuggerDisplay("{OperationIdentity.OperationIdentity.Description} для {string.Join(\", \", OperationIdentity.Entities)}")]
    public sealed class EntityOperationMapping<TEntity> where TEntity : class, IEntity, IEntityKey
    {
        private readonly StrictOperationIdentity _operationIdentity;
        private readonly Func<IFinder, IEnumerable<long>, IQueryable<TEntity>> _selectExpression;

        private EntityOperationMapping(StrictOperationIdentity operationIdentity, Func<IFinder, IEnumerable<long>, IQueryable<TEntity>> selectExpression)
        {
            _operationIdentity = operationIdentity;
            _selectExpression = selectExpression;
        }

        public StrictOperationIdentity OperationIdentity
        {
            get { return _operationIdentity; }
        }

        public Func<IFinder, IEnumerable<long>, IQueryable<TEntity>> SelectExpression
        {
            get { return _selectExpression; }
        }

        public static ForEntityMapping<TEntity> ForEntity(EntityName entityName)
        {
            return new ForEntityMapping<TEntity>(entityName);
        }

        public class ForEntityMapping<TEntity> where TEntity : class, IEntity, IEntityKey
        {
            private readonly EntityName _entityName;
            private readonly List<StrictOperationIdentity> _operationIdentities = new List<StrictOperationIdentity>();

            public ForEntityMapping(EntityName entityName)
            {
                _entityName = entityName;
            }

            public ForEntityMapping<TEntity> NonCoupledOperation<TOperationIdentity>()
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
            {
                _operationIdentities.Add(new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(_entityName)));
                return this;
            }

            public ForEntityMapping<TEntity> Operation<TOperationIdentity>()
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            {
                var operationIdentity = new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(_entityName));
                _operationIdentities.Add(operationIdentity);
                return this;
            }

            public ForEntityMapping<TEntity> Operation<TOperationIdentity>(EntityName entityName1, EntityName entityName2)
                where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            {
                var operationIdentity = new StrictOperationIdentity(OperationIdentityBase<TOperationIdentity>.Instance, new EntitySet(entityName1, entityName2));
                _operationIdentities.Add(operationIdentity);
                return this;
            }

            public IEnumerable<EntityOperationMapping<TEntity>> Use(Func<IFinder, IEnumerable<long>, IQueryable<TEntity>> selectExpression)
            {
                return _operationIdentities.Select(operationIdentity => new EntityOperationMapping<TEntity>(operationIdentity, selectExpression)).ToArray();
            }
        }
    }
}
