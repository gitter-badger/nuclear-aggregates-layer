using System;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

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
            var entities = new EntitySet(new[] { typeof(TEntity) }.AsEntityTypes());

            return BuildOperationAccessRequirement(action, entities);
        }

        public static IOperationAccessRequirement ForOperation<TIdentity, TEntity1, TEntity2>(Action<OperationAccessRequirement<TIdentity>> action)
            where TIdentity : OperationIdentityBase<TIdentity>, IEntitySpecificOperationIdentity, new() where TEntity1 : IEntity, IEntityKey
            where TEntity2 : IEntity, IEntityKey
        {
            var entities = new EntitySet(new[] { typeof(TEntity1), typeof(TEntity2) }.AsEntityTypes());

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