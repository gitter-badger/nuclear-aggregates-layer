using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.PrintRegional;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BLCore.WCF.MoDi
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PrintRegionalApplicationService : IPrintRegionalApplicationService
    {
        private readonly IPrintRegionalOrderService _printRegionalOrderService;

        public PrintRegionalApplicationService(IUserContext userContext, IPrintRegionalOrderService printRegionalOrderService)
        {
            _printRegionalOrderService = printRegionalOrderService;

            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public PrintRegionalOrdersResponse PrintRegionalOrder(long orderId)
        {
            return _printRegionalOrderService.PrintRegionalOrder(orderId);
        }

        public PrintRegionalOrdersResponse PrintRegionalOrders(long organizationId, DateTime startDate, DateTime endDate)
        {
            return _printRegionalOrderService.PrintRegionalOrder(organizationId, startDate, endDate);
        }
    }
}
