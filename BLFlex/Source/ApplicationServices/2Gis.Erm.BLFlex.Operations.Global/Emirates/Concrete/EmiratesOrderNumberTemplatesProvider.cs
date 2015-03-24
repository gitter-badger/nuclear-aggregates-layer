using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete
{
    public sealed class EmiratesOrderNumberTemplatesProvider : IOrderNumberTemplatesProvider
    {
        public const string OrderNumberTemplate = "Q_{0}-{1}-{2}";

        public string GetTemplate(OrderType orderType)
        {
            return OrderNumberTemplate;
        }

        public string GetRegionalTemplate()
        {
            throw new NotSupportedException(BLResources.RegionalOrdersAreNotSupported);
        }
    }
}