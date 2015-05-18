using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCurrency : EntityTypeBase<EntityTypeCurrency>
    {
        public override int Id
        {
            get { return EntityTypeIds.Currency; }
        }

        public override string Description
        {
            get { return "Currency"; }
        }
    }
}