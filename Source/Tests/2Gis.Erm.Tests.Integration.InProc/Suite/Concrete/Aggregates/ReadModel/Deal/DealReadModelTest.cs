using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Deal
{
    public class DealReadModelTest : IIntegrationTest
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IAppropriateEntityProvider<Platform.Model.Entities.Erm.Deal> _dealEntityProvider;

        public DealReadModelTest(IDealReadModel dealReadModel, IAppropriateEntityProvider<Platform.Model.Entities.Erm.Deal> dealEntityProvider)
        {
            _dealReadModel = dealReadModel;
            _dealEntityProvider = dealEntityProvider;
        }

        public ITestResult Execute()
        {
            var date = DateTime.UtcNow.AddMonths(-1);
            var releaseNumber = date.ToAbsoluteReleaseNumber();
            var dealsWithAfterSaleService = _dealEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Platform.Model.Entities.Erm.Deal>() &&
                                                                   new FindSpecification<Platform.Model.Entities.Erm.Deal>(
                                                                       x =>
                                                                       x.AfterSaleServiceActivities.Any(
                                                                           assa =>
                                                                           assa.AfterSaleServiceType == (byte)AfterSaleServiceType.ASS1 &&
                                                                           assa.AbsoluteMonthNumber == releaseNumber)), 5);

            if (!dealsWithAfterSaleService.Any())
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var afterSaleServiceActivity = _dealReadModel.GetAfterSaleService(dealsWithAfterSaleService.First().ReplicationCode, date, AfterSaleServiceType.ASS1);
            var deal = _dealReadModel.GetDeal(dealsWithAfterSaleService.First().Id);
            var dealByGuid = _dealReadModel.GetDeal(dealsWithAfterSaleService.First().ReplicationCode);
            var infoForActualizeProfits = _dealReadModel.GetInfoForActualizeProfits(dealsWithAfterSaleService.Select(x => x.Id).ToArray(), true);
            var dealActualizeDuringWithdrawalDtos = _dealReadModel.GetInfoForWithdrawal(dealsWithAfterSaleService.Select(x => x.Id).ToArray());
            _dealReadModel.HasOrders(dealsWithAfterSaleService.First().Id);

            return new object[]
                {
                    afterSaleServiceActivity,
                    deal,
                    dealByGuid,
                    infoForActualizeProfits,
                    dealActualizeDuringWithdrawalDtos
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}