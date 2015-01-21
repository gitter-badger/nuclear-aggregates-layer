using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdditionalFirmService : EntityTypeBase<EntityTypeAdditionalFirmService>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdditionalFirmService; }
        }

        public override string Description
        {
            get { return "AdditionalFirmService"; }
        }
    }
}