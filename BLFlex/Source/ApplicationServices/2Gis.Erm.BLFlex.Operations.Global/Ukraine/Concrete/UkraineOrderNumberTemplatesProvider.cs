﻿using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete
{
    public sealed class UkraineOrderNumberTemplatesProvider : IOrderNumberTemplatesProvider
    {
        public const string OrderNumberTemplate = "БЗ_{0}-{1}-{2}";

        public string GetTemplate(OrderType orderType)
        {
            return OrderNumberTemplate;
        }

        public string GetRegionalTemplate()
        {
            return OrderNumberTemplate;
        }
    }
}