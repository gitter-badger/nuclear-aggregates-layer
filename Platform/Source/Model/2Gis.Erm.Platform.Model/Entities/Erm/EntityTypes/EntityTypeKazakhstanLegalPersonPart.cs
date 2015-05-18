using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeKazakhstanLegalPersonPart : EntityTypeBase<EntityTypeKazakhstanLegalPersonPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.KazakhstanLegalPersonPart; }
        }

        public override string Description
        {
            get { return "KazakhstanLegalPersonPart"; }
        }
    }
}