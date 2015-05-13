using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, MultiCultureListOrderDto> OrdersForMulticultureGridView()
            {
                return new SelectSpecification<Order, MultiCultureListOrderDto>(
                    order => new MultiCultureListOrderDto
                    {
                        Id = order.Id,
                        Number = order.Number,
                        CreatedOn = order.CreatedOn,
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
                        EndDistributionDateFact = order.EndDistributionDateFact,
                        LegalPersonId = order.LegalPersonId,
                        LegalPersonName = order.LegalPerson.LegalName,
                        OwnerCode = order.OwnerCode,
                        BargainId = order.BargainId,
                        WorkflowStepEnum = order.WorkflowStepId,
                        PayablePlan = order.PayablePlan,
                        PayableFact = order.PayableFact,
                        AmountWithdrawn = order.AmountWithdrawn,
                        ModifiedOn = order.ModifiedOn,
                        DiscountPercent = order.DiscountPercent,
                        AccountId = order.AccountId,
                        DealId = order.DealId,
                        IsActive = order.IsActive,
                        IsDeleted = order.IsDeleted,
                        IsTerminated = order.IsTerminated,
                        InspectorCode = order.InspectorCode,
                        HasDocumentsDebtEnum = order.HasDocumentsDebt,
                        OrderTypeEnum = order.OrderType,
                        TerminationReasonEnum = order.TerminationReason,
                        OrderType = (order.OrderType).ToStringLocalizedExpression(),
                        OwnerName = null,
                        WorkflowStep = (order.WorkflowStepId).ToStringLocalizedExpression(),
                        PaymentMethod = (order.PaymentMethod).ToStringLocalizedExpression(),
                    });
            }
        }
    }
}
