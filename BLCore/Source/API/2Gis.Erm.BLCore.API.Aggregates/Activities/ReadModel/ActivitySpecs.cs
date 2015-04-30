using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public static class ActivitySpecs
    {
        public static class Find
        {
            public static FindSpecification<TReferencedObject> ByReferencedObject<TEntity, TReferencedObject>(IEntityType entityName, long entityId)
                where TEntity : IEntity
                where TReferencedObject : EntityReference<TEntity>, IEntity
            {
                return new FindSpecification<TReferencedObject>(x => x.TargetEntityTypeId == entityName.Id && x.TargetEntityId == entityId);
            }
        }
    }
}