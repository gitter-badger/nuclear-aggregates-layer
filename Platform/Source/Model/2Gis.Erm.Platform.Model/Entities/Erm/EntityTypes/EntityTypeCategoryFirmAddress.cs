using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCategoryFirmAddress : EntityTypeBase<EntityTypeCategoryFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryFirmAddress; }
        }

        public override string Description
        {
            get { return "CategoryFirmAddress"; }
        }
    }
}