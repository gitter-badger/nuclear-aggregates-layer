using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, OrderGridViewDto> OrdersForGridView()
            {
                return new SelectSpecification<Order, OrderGridViewDto>(
                    order => new OrderGridViewDto
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
                        OwnerCode = order.OwnerCode,
                        BargainId = order.BargainId,
                        BargainNumber = order.Bargain.Number,
                        WorkflowStepId = order.WorkflowStepId,
                        PayablePlan = order.PayablePlan,
                        AmountWithdrawn = order.AmountWithdrawn,
                        ModifiedOn = order.ModifiedOn
                    });
            }
        }
    }
}
