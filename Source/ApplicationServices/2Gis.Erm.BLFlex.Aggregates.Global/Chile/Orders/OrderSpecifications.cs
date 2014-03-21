using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, ChileListOrderDto> OrdersForCzechGridView()
            {
                return new SelectSpecification<Order, ChileListOrderDto>(
                    x => new ChileListOrderDto
                    {
                        Id = x.Id,
                        OrderNumber = x.Number,
                        CreatedOn = x.CreatedOn,
                        FirmId = x.FirmId,
                        FirmName = x.Firm.Name,
                        ClientId = x.Firm.Client.Id,
                        ClientName = x.Firm.Client.Name,
                        DestOrganizationUnitId = x.DestOrganizationUnitId,
                        DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                        SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                        SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                        BeginDistributionDate = x.BeginDistributionDate,
                        EndDistributionDatePlan = x.EndDistributionDatePlan,
                        EndDistributionDateFact = x.EndDistributionDateFact,
                        LegalPersonId = x.LegalPersonId,
                        LegalPersonName = x.LegalPerson.LegalName,
                        PaymentMethodEnum = (PaymentMethod)x.PaymentMethod,
                        OwnerCode = x.OwnerCode,
                        BargainId = x.BargainId,
                        WorkflowStepEnum = (OrderState)x.WorkflowStepId,
                        PayablePlan = x.PayablePlan,
                        PayableFact = x.PayableFact,
                        AmountWithdrawn = x.AmountWithdrawn,
                        ModifiedOn = x.ModifiedOn,
                        DiscountPercent = x.DiscountPercent,
                        AccountId = x.AccountId,
                        DealId = x.DealId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        IsTerminated = x.IsTerminated,
                        HasDocumentsDebtEnum = (DocumentsDebt)x.HasDocumentsDebt,
                        OrderTypeEnum = (OrderType)x.OrderType,
                        TerminationReasonEnum = (OrderTerminationReason)x.TerminationReason,
                        OrderType = null,
                        OwnerName = null,
                        WorkflowStep = null,
                        PaymentMethod = null,
                    });
            }
        }
    }
}
