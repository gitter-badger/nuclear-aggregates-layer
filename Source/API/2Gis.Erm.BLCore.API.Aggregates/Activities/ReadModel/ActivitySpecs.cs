using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public static class ActivitySpecs
    {
        public static class Find
        {
            public static FindSpecification<TReferencedObject> ByReferencedObject<TEntity, TReferencedObject>(EntityName entityName, long entityId)
                where TEntity : IEntity
                where TReferencedObject : EntityReference<TEntity>, IEntity
            {
                return new FindSpecification<TReferencedObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId);
            }
        }
    }
}