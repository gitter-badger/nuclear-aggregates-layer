using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class MoDiPriceCostsForSubPositionsProvider : IPriceCostsForSubPositionsProvider
    {
        private readonly IClientProxyFactory _clientProxyFactory;

        public MoDiPriceCostsForSubPositionsProvider(IClientProxyFactory clientProxyFactory)
        {
            _clientProxyFactory = clientProxyFactory;
        }

        public IReadOnlyCollection<PriceCostDto> GetPriceCostsForSubPositions(long parentPositionId, long priceId)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IWithdrawalInfoApplicationService, WSHttpBinding>();
            return clientProxy.Execute(action => action.GetPriceCostsForSubPositions(parentPositionId, priceId))
                              .Select(x => new PriceCostDto
                                               {
                                                   Cost = x.Cost,
                                                   Platform = x.Platform,
                                                   PositionId = x.PositionId
                                               }).ToArray();
        }
    }
}