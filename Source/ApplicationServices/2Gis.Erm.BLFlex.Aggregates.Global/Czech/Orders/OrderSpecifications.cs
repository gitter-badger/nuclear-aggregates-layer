using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, CzechOrderGridViewDto> OrdersForCzechGridView()
            {
                return new SelectSpecification<Order, CzechOrderGridViewDto>(
                    order => new CzechOrderGridViewDto
                    {
                        Id = order.Id,
                        Number = order.Number,
                        FirmId = order.FirmId,
                        FirmName = order.Firm.Name,
                        ClientId = order.Firm.Client.Id,
                        ClientName = order.Firm.Client.Name,
                        DestOrganizationUnitId = order.DestOrganizationUnitId,
                        DestOrganizationUnitName = order.DestOrganizationUnit.Name,
                        SourceOrganizationUnitId = order.SourceOrganizationUnitId,
                        SourceOrganizationUnitName = order.SourceOrganizationUnit.Name,
                        BeginDistributionDate = order.BeginDistributionDate,
                        EndDistributionDatePlan = order.EndDistributionDatePlan,
                        LegalPersonId = order.LegalPersonId,
                        LegalPersonName = order.LegalPerson.LegalName,
                        PaymentMethod = order.PaymentMethod,
                        OwnerCode = order.OwnerCode,
                        BargainId = order.BargainId,
                        BargainNumber = order.Bargain.Number,
                        WorkflowStepId = order.WorkflowStepId,
                        PayablePlan = order.PayablePlan,
                        PayableFact = order.PayableFact,
                        AmountWithdrawn = order.AmountWithdrawn,
                        ModifiedOn = order.ModifiedOn,
                        OrderType = order.OrderType,
                        DiscountPercent = order.DiscountPercent
                    });
            }
        }
    }
}
