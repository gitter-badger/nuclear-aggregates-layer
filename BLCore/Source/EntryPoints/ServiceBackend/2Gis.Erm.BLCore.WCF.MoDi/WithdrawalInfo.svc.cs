using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.MoDi.Settings;
using DoubleGis.Erm.BLCore.MoDi.Configurations;

namespace DoubleGis.Erm.BLCore.WCF.MoDi
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class WithdrawalInfoApplicationService : IWithdrawalInfoApplicationService
    {
        private readonly IMoneyDistributionSettings _moneyDistributionSettings;

        public WithdrawalInfoApplicationService(IMoneyDistributionSettings moneyDistributionSettings)
        {
            _moneyDistributionSettings = moneyDistributionSettings;
        }

        // нет смысла в отдельной операции, слишком простой контракт
        public PriceCostInfo[] GetPriceCostsForSubPositions(long parentPositionId, long priceId)
        {
            var priceCostInfos = PackagePositionsSettings.Instance
                .PackagePositions.Cast<PackagePosition>()
                .Where(x => x.Id == parentPositionId && x.PriceId == priceId)
                .SelectMany(x => x.ChildPositions.Cast<ConfigPosition>())
                .Select(x => new
                {
                    ConfigPosition = x,
                    Platform = _moneyDistributionSettings.PlatformMap[x.Platform],
                })
                .GroupBy(x => new { x.Platform, x.ConfigPosition.Id })
                .Select(x => new PriceCostInfo
                {
                    PositionId = x.Key.Id,
                    Platform = x.Key.Platform,
                    Cost = x.Sum(y => y.ConfigPosition.Cost),
                })
                .ToArray();

            return priceCostInfos;
        }
    }
}