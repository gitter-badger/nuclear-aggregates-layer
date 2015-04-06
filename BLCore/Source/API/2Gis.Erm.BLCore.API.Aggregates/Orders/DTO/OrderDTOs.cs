using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderUsageDto
    {
        public Order Order { get; set; }
        public bool AnyLocks { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderDiscountsDto
    {
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class RecalculatedOrderPositionDataDto
    {
        public int ShipmentPlan { get; set; }
        public decimal PayablePrice { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal PayablePlanWoVat { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderPayablePlanInfo
    {
        public long OrderId { get; set; }
        public decimal PayablePlan { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class VatRateDetailsDto
    {
        public decimal VatRate { get; set; }
        public bool ShowVat { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderInfoToGetInitPayments
    {
        public int ReleaseCountPlan { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public DateTime SignupDate { get; set; }
        public decimal PayablePlan { get; set; }
        public bool IsOnRegistration { get; set; }
        public int BillsCount { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class OrderWithdrawalInfo
    {
        public long OrderId { get; set; }
        public int AccountDetailsCount { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public decimal AmountToWithdraw { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class OrderReleaseInfo
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public long? AccountId { get; set; }
        public long PriceId { get; set; }
        public decimal AmountToWithdrawSum { get; set; }

        public IEnumerable<OrderPositionReleaseInfo> OrderPositions { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class OrderPositionReleaseInfo
    {
        public long OrderPositionId { get; set; }
        public bool IsPlannedProvision { get; set; }
        public decimal AmountToWithdraw { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class OrderInfoToCheckOrderBeginDistributionDate
    {
        public long OrderId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestinationOrganizationUnitId { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderFinancialInfo
    {
        public decimal? DiscountSum { get; set; }
        public short ReleaseCountFact { get; set; }
        public bool DiscountInPercent { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderStateValidationInfo
    {
        public long? LegalPersonId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public DocumentsDebt HasDocumentsDebt { get; set; }
        public bool AnyPositions { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderPositionAdvertisementLinksDto
    {
        public IEnumerable<OrderPositionAdvertisement> AdvertisementLinks { get; set; }
        public PositionBindingObjectType BindingType { get; set; }
        public OrderState OrderWorkflowState { get; set; }
        public long OrderId { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderWithBillsDto
    {
        public Order Order { get; set; }
        public IEnumerable<Bill> Bills { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class SubPositionDto
    {
        public long PositionId { get; set; }
        public long PlatformId { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderForProlongationDto
    {
        public long OrderId { get; set; }
        public OrderType OrderType { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long FirmId { get; set; }
        public int ReleaseCountFact { get; set; }
        public IEnumerable<OrderPositionForProlongationDto> Positions { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderRepairOutdatedOrderPositionDto
    {
        public IEnumerable<OrderReleaseTotal> ReleaseTotals { get; set; }
        public IEnumerable<OrderPositionDto> OrderPositions { get; set; }
        
        public sealed class OrderPositionDto
    {
        public OrderPosition OrderPosition { get; set; }
        public string PositionName { get; set; }
        public PricePosition PricePosition { get; set; }
        public IEnumerable<OrderPositionAdvertisement> Advertisements { get; set; }
        public IEnumerable<AdvertisementDescriptor> ClonedAdvertisements { get; set; }
            
            public IEnumerable<OrderReleaseWithdrawalDto> ReleaseWithdrawals { get; set; }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderPositionForProlongationDto
    {
        public long PositionId { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public int Amount { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderPositionWithAdvertisementsDto
    {
        public OrderPosition OrderPosition { get; set; }
        public IEnumerable<OrderPositionAdvertisement> Advertisements { get; set; }
        public PositionBindingObjectType BindingObjectType { get; set; }
        public bool IsComposite { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderDeactivationPosibility
    {
        public string EntityCode { get; set; }
        public bool IsDeactivationAllowed { get; set; }
        public string DeactivationConfirmation { get; set; }
        public string DeactivationDisallowedReason { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderPositionSalesModelDto
    {
        public long OrderPositionId { get; set; }
        public SalesModel SalesModel { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderDtoToCheckPossibilityOfOrderPositionCreation
    {
        public long OrderId { get; set; }
        public long FirmId { get; set; }
        public IEnumerable<OrderPositionSalesModelDto> OrderPositions { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class RecipientDto
    {
        public long FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ContactClientEmail { get; set; }
        public string ContactClientSex { get; set; }
        public string ContactClientName { get; set; }
        public bool IsClientActually { get; set; }
        public bool IsOrdersOfWebAPI { get; set; }
        public string CuratorEmail { get; set; }
        public string BranchName { get; set; }
        public string LegalEntityBranchDirectorName { get; set; }
        public string LegalEntityBranchEmail { get; set; }

        public XElement ToXml()
        {
            var xml = new XElement("Recipient");

            xml.Add(new XAttribute("BranchName", BranchName));
            if (!string.IsNullOrWhiteSpace(ContactClientName))
            {
                xml.Add(new XAttribute("ContactClientName", ContactClientName));
            }

            if (!string.IsNullOrWhiteSpace(ContactClientEmail))
            {
                xml.Add(new XAttribute("ContactClientEmail", ContactClientEmail));
            }

            if (!string.IsNullOrWhiteSpace(ContactClientSex) && ContactClientSex != Gender.None.ToString())
            {
                xml.Add(new XAttribute("ContactClientSex", ContactClientSex));
            }

            if (!string.IsNullOrWhiteSpace(CuratorEmail))
            {
                xml.Add(new XAttribute("CuratorEmail", CuratorEmail));
            }

            xml.Add(new XAttribute("FirmCode", FirmCode));
            xml.Add(new XAttribute("FirmName", FirmName));

            xml.Add(new XAttribute("IsClientActually", IsClientActually));
            xml.Add(new XAttribute("IsOrdersOfWebAPI", IsOrdersOfWebAPI));

            if (!string.IsNullOrWhiteSpace(LegalEntityBranchDirectorName))
            {
                xml.Add(new XAttribute("LegalEntityBranchDirectorName", LegalEntityBranchDirectorName));
            }

            if (!string.IsNullOrWhiteSpace(LegalEntityBranchEmail))
            {
                xml.Add(new XAttribute("LegalEntityBranchEmail", LegalEntityBranchEmail));
            }

            return xml;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderCompletionState
    {
        public bool LegalPerson { get; set; }
        public bool BranchOfficeOrganizationUnit { get; set; }
    }

    public sealed class ReleaseNumbersDto
    {
        public int BeginReleaseNumber { get; set; }
        public int EndReleaseNumberPlan { get; set; }
        public int EndReleaseNumberFact { get; set; }
    }

    public sealed class DistributionDatesDto
    {
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
    }
}