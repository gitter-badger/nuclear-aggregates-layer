using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public static class DealSpecs
    {
        public static class Deals
        {
            public static class Find
            {
                public static FindSpecification<Deal> ForClient(long clientId)
                {
                    return new FindSpecification<Deal>(x => x.ClientId == clientId);
                }
            }

            public static class Select
            {
                public static SelectSpecification<Deal, DealActualizeProfitDto> ActualizeProfitInfo(bool processActuallyReceivedProfit)
                {
                    return new SelectSpecification<Deal, DealActualizeProfitDto>(
                        d => new DealActualizeProfitDto
                            {
                                Deal = d,
                                ActuallyReceivedProfit =
                                    !processActuallyReceivedProfit
                                        ? (decimal?)null
                                        : d.Orders
                                           .Where(o => !o.IsDeleted && o.WorkflowStepId != (int)OrderState.Rejected)
                                           .Sum(o => o.AmountWithdrawn),
                                RemainingExpectedProfit =
                                    d.Orders
                                     .Where(o => !o.IsDeleted && o.WorkflowStepId != (int)OrderState.Rejected)
                                     .Sum(
                                          o =>
                                          (decimal?)(o.OrderReleaseTotals
                                                         // находим, суммы к списанию, оставшиеся до конца размещения заказа, исключая те, за периоды действия которых уже выполнены списания
                                                      .Where(ort =>
                                                             ort.ReleaseNumber >= o.BeginReleaseNumber
                                                             && ort.ReleaseNumber <= o.EndReleaseNumberFact
                                                             && !o.Locks
                                                                  .Where(l => !l.IsActive && !l.IsDeleted)
                                                                  .Any(l => l.PeriodStartDate == ort.ReleaseBeginDate && l.PeriodEndDate == ort.ReleaseEndDate))
                                                      .Sum(ort => (decimal?)ort.AmountToWithdraw) ?? 0M)) ?? 0M
                            });
                }
            }
        }
    }
}