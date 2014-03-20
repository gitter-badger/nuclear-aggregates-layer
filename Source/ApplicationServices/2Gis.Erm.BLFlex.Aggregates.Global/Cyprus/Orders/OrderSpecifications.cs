﻿using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Orders
{
    public static class OrderSpecifications
    {
        public static class Select
        {
            public static ISelectSpecification<Order, CyprusListOrderDto> OrdersForCyprusGridView()
            {
                return new SelectSpecification<Order, CyprusListOrderDto>(
                    x => new CyprusListOrderDto
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
                        BargainNumber = x.Bargain.Number,
                        WorkflowStepEnum = (OrderState)x.WorkflowStepId,
                        PayablePlan = x.PayablePlan,
                        AmountWithdrawn = x.AmountWithdrawn,
                        ModifiedOn = x.ModifiedOn,
                        AccountId = x.AccountId,
                        DealId = x.DealId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        IsTerminated = x.IsTerminated,
                        HasDocumentsDebtEnum = (DocumentsDebt)x.HasDocumentsDebt,
                        OrderTypeEnum = (OrderType)x.OrderType,
                        TerminationReasonEnum = (OrderTerminationReason)x.TerminationReason,
                        OwnerName = null,
                        WorkflowStep = null,
                        PaymentMethod = null,
                    });
            }
        }
    }
}
