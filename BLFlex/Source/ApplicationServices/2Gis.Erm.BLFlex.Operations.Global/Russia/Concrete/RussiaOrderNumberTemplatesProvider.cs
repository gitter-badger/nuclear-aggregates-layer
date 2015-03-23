using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete
{
    public sealed class RussiaOrderNumberTemplatesProvider : IOrderNumberTemplatesProvider
    {
        public const string OrderNumberTemplate = "БЗ_{0}-{1}-{2}";
        public const string RegionalOrderNumberTemplate = "ОФ_{0}-{1}-{2}";
        public const string AdvAgenciesTemplatePostfix = "-РА";

        public string GetTemplate(OrderType orderType)
        {
            if (orderType == OrderType.AdvertisementAgency)
            {
                return OrderNumberTemplate + AdvAgenciesTemplatePostfix;
            }

            return OrderNumberTemplate;
        }

        public string GetRegionalTemplate()
        {
            return RegionalOrderNumberTemplate;
        }
    }
}