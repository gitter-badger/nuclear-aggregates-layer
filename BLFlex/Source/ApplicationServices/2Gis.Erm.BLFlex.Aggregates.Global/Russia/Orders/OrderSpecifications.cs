using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, ListOrderDto> OrdersForGridView()
            {
                return new SelectSpecification<Order, ListOrderDto>(
                    x => new ListOrderDto
                    {
                        Id = x.Id,
                        Number = x.Number,
                        CreatedOn = x.CreatedOn,
                        FirmId = x.FirmId,
                        FirmName = x.Firm.Name,
                        ClientId = x.Firm.Client.Id,
                        DestOrganizationUnitId = x.DestOrganizationUnitId,
                        DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                        SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                        SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                        BeginDistributionDate = x.BeginDistributionDate,
                        EndDistributionDatePlan = x.EndDistributionDatePlan,
                        EndDistributionDateFact = x.EndDistributionDateFact,
                        LegalPersonId = x.LegalPersonId,
                        LegalPersonName = x.LegalPerson.LegalName,
                        OwnerCode = x.OwnerCode,
                        BargainId = x.BargainId,
                        BargainNumber = x.Bargain.Number,
                        WorkflowStepEnum = x.WorkflowStepId,
                        PayablePlan = x.PayablePlan,
                        AmountWithdrawn = x.AmountWithdrawn,
                        ModifiedOn = x.ModifiedOn,
                        InspectorCode = x.InspectorCode,
                        AccountId = x.AccountId,
                        DealId = x.DealId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        IsTerminated = x.IsTerminated,
                        HasDocumentsDebtEnum = x.HasDocumentsDebt,
                        OrderTypeEnum = x.OrderType,
                        TerminationReasonEnum = x.TerminationReason,
                        OwnerName = null,
                        WorkflowStep = (x.WorkflowStepId).ToStringLocalizedExpression(),
                    });
            }
        }
    }
}
