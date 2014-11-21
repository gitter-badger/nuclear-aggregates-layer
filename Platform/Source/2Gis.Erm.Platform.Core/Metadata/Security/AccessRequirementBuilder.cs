using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public sealed class AccessRequirementBuilder
    {
        public static IOperationAccessRequirement ForOperation<TIdentity>(Action<OperationAccessRequirement<TIdentity>> action)
            where TIdentity : OperationIdentityBase<TIdentity>, INonCoupledOperationIdentity, new()
        {
            var entities = EntitySet.Create.NonCoupled;

            return BuildOperationAccessRequirement(action, entities);
        }

        public static IOperationAccessRequirement ForOperation<TIdentity, TEntity>(Action<OperationAccessRequirement<TIdentity>> action)
            where TIdentity : OperationIdentityBase<TIdentity>, IEntitySpecificOperationIdentity, new() where TEntity : IEntity, IEntityKey
        {
            var entities = new EntitySet(new[] { typeof(TEntity) }.AsEntityNames());

            return BuildOperationAccessRequirement(action, entities);
        }

        public static IOperationAccessRequirement ForOperation<TIdentity, TEntity1, TEntity2>(Action<OperationAccessRequirement<TIdentity>> action)
            where TIdentity : OperationIdentityBase<TIdentity>, IEntitySpecificOperationIdentity, new() where TEntity1 : IEntity, IEntityKey
            where TEntity2 : IEntity, IEntityKey
        {
            var entities = new EntitySet(new[] { typeof(TEntity1), typeof(TEntity2) }.AsEntityNames());

            return BuildOperationAccessRequirement(action, entities);
        }

        private static IOperationAccessRequirement BuildOperationAccessRequirement<TIdentity>(
            Action<OperationAccessRequirement<TIdentity>> action, 
            EntitySet entities) where TIdentity : OperationIdentityBase<TIdentity>, new()
        {
            var operationIdentity = OperationIdentityBase<TIdentity>.Instance;
            var strinctOperationIdentity = new StrictOperationIdentity(operationIdentity, entities);
            var requirement = new OperationAccessRequirement<TIdentity>(strinctOperationIdentity);
            action.Invoke(requirement);
            return requirement;
        }
    }
}