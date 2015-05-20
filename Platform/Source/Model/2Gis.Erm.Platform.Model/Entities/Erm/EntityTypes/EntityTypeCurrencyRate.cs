using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCurrencyRate : EntityTypeBase<EntityTypeCurrencyRate>
    {
        public override int Id
        {
            get { return EntityTypeIds.CurrencyRate; }
        }

        public override string Description
        {
            get { return "CurrencyRate"; }
        }
    }
}