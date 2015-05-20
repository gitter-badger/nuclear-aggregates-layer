using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDenialReason : EntityTypeBase<EntityTypeDenialReason>
    {
        public override int Id
        {
            get { return EntityTypeIds.DenialReason; }
        }

        public override string Description
        {
            get { return "DenialReason"; }
        }
    }
}