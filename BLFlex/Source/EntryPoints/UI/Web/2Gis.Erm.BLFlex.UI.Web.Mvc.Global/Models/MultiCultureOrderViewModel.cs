using System;
using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureOrderViewModel : EntityViewModelBase<Order>,
                                                     INumberAspect,
                                                     IOrderWorkflowLockableAspect,
                                                     IOrderDirectionAspect,
                                                     IInspectorAspect,
                                                     IOrderDatesAspect,
                                                     IOrderWorkflowAspect,
                                                     IOrderValidationServiceAspect,
                                                     IOrderSecurityAspect,
                                                     ITerminatableAspect,
                                                     ICzechAdapted,
                                                     ICyprusAdapted,
                                                     IChileAdapted,
                                                     IUkraineAdapted,
                                                     IEmiratesAdapted,
                                                     IKazakhstanAdapted
    {
        long? IOrderDirectionAspect.SourceOrganizationUnitKey
        {
            get { return SourceOrganizationUnit.Key; }
        }

        long? IOrderDirectionAspect.DestinationOrganizationUnitKey
        {
            get { return DestinationOrganizationUnit.Key; }
        }

        string IOrderDirectionAspect.SourceOrganizationUnitValue
        {
            get { return SourceOrganizationUnit.Value; }
        }

        string IOrderDirectionAspect.DestinationOrganizationUnitValue
        {
            get
            {
                return DestinationOrganizationUnit.Value;
            }
        }

        long? IInspectorAspect.InspectorKey
        {
            get { return Inspector.Key; }
        }

        string IInspectorAspect.InspectorValue
        {
            get { return Inspector.Value; }
            set { Inspector.Value = value; }
        }

        OrderState IOrderWorkflowAspect.WorkflowStepId
        {
            get { return (OrderState)WorkflowStepId; }
        }

        [Dependency(DependencyType.Hidden, "RegionalNumber", @"Ext.getDom('Id').value==0 ||
                                                             (!Ext.getCmp('SourceOrganizationUnit').getValue() || !Ext.getCmp('DestinationOrganizationUnit').getValue()) ||
                                                             (Ext.getDom('Id').value!==0 && Ext.getCmp('SourceOrganizationUnit').getValue().id==Ext.getCmp('DestinationOrganizationUnit').getValue().id)||
                                                             Ext.getDom('ShowRegionalAttributes').checked==false")]
        [Dependency(DependencyType.Required, "OrderNumber", @"Ext.getDom('Id').value != 0")]
        public override long Id
        {
            get { return base.Id; }

            set { base.Id = value; }
        }

        [StringLengthLocalized(200)]
        [DisplayNameLocalized("OrderNumber")]
        public string Number { get; set; }

        [StringLengthLocalized(200)]
        public string RegionalNumber { get; set; }

        [RequiredLocalized]
        public LookupField Firm { get; set; }

        [Dependency(DependencyType.ReadOnly, "SourceOrganizationUnit",
            "(this.value && this.value.toLowerCase()=='true')||(Ext.getDom('Id').value=='0'&&Ext.getCmp('SourceOrganizationUnit').getValue()!=undefined)")]
        [Dependency(DependencyType.ReadOnly, "DestinationOrganizationUnit", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "LegalPerson", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeOrganizationUnit", "(this.value && this.value.toLowerCase()=='true')")]
        [Dependency(DependencyType.ReadOnly, "Firm", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.Disable, "OrderType",
            "(this.value && this.value.toLowerCase()=='true') || Ext.getDom('CanEditOrderType').value.toLowerCase()=='false'")]
        public bool MakeReadOnly
        {
            get { return HasAnyOrderPosition || !IsActive; }
        }

        public bool ShowRegionalAttributes { get; set; }

        public bool HasAnyOrderPosition { get; set; }

        public bool HasDestOrganizationUnitPublishedPrice { get; set; }

        public bool CanEditOrderType { get; set; }
        public bool HasOrderDocumentsDebtChecking { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeOrganizationUnit", "!Ext.getCmp('SourceOrganizationUnit').getValue()")]
        [Dependency(DependencyType.Hidden, "RegionalNumber", @"Ext.getDom('Id').value==0 ||
                                                             (!Ext.getCmp('SourceOrganizationUnit').getValue() || !Ext.getCmp('DestinationOrganizationUnit').getValue()) ||
                                                             (Ext.getDom('Id').value!==0 && Ext.getCmp('SourceOrganizationUnit').getValue().id==Ext.getCmp('DestinationOrganizationUnit').getValue().id)||
                                                             Ext.getDom('ShowRegionalAttributes').checked==false")]
        public LookupField SourceOrganizationUnit { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Hidden, "RegionalNumber", @"Ext.getDom('Id').value==0 ||
                                                             (!Ext.getCmp('SourceOrganizationUnit').getValue() || !Ext.getCmp('DestinationOrganizationUnit').getValue()) ||
                                                             (Ext.getDom('Id').value!==0 && Ext.getCmp('SourceOrganizationUnit').getValue().id==Ext.getCmp('DestinationOrganizationUnit').getValue().id)||
                                                             Ext.getDom('ShowRegionalAttributes').checked==false")]
        public LookupField DestinationOrganizationUnit { get; set; }

        [DisplayNameLocalized("BranchOfficeOrganizationUnitName")]
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        public LookupField LegalPerson { get; set; }
        public LookupField LegalPersonProfile { get; set; }

        public LookupField Deal { get; set; }

        public long? DealCurrencyId { get; set; }

        [CheckDayOfMonth(CheckDayOfMonthType.FirstDay, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "RequiredFirstDayOfMonthMessage")]
        [DisplayNameLocalized("BeginReleaseDate")]
        [CustomClientValidation("validateBeginDistributionDate", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "IncorrectBeginDistributionDate")]
        public DateTime BeginDistributionDate { get; set; }

        [CheckDayOfMonth(CheckDayOfMonthType.LastDay, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "RequiredLastDayOfMonthMessage")]
        [DisplayNameLocalized("EndPlanReleaseDate")]
        [GreaterOrEqualThan("BeginDistributionDate", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "EndDateMustBeGreaterThenBeginDate")]
        public DateTime EndDistributionDatePlan { get; set; }

        [DisplayNameLocalized("EndFactReleaseDate")]
        public DateTime EndDistributionDateFact { get; set; }

        public DateTime SignupDate { get; set; }

        public int BeginReleaseNumber { get; set; }

        [DisplayNameLocalized("EndPlanReleaseNumber")]
        public int EndReleaseNumberPlan { get; set; }

        [DisplayNameLocalized("EndFactReleaseNumber")]
        public int EndReleaseNumberFact { get; set; }

        [DisplayNameLocalized("PlanReleaseCount")]
        [OnlyDigitsLocalized]
        [Range(1, 12, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "ReleaseCountPlanRangeMessage")]
        public short ReleaseCountPlan { get; set; }

        [DisplayNameLocalized("FactReleaseCount")]
        public short ReleaseCountFact { get; set; }

        public LookupField Currency { get; set; }

        public int PreviousWorkflowStepId { get; set; }

        [Dependency(DependencyType.Required, "Currency", "this.value == 2")]
        [Dependency(DependencyType.Required, "ReleaseCountPlan", "this.value == 2")]
        [Dependency(DependencyType.Required, "EndReleaseNumberPlan", "this.value == 2")]
        [Dependency(DependencyType.Required, "BeginReleaseNumber", "this.value == 2")]
        [Dependency(DependencyType.Required, "LegalPerson", "this.value == 2")]
        [Dependency(DependencyType.Required, "BranchOfficeOrganizationUnit", "this.value == 2")]
        [Dependency(DependencyType.Required, "Inspector", "this.value == 2")]
        [Dependency(DependencyType.Disable, "PaymentMethod", "this.value >= 2")]
        public int WorkflowStepId { get; set; }

        public string AvailableSteps { get; set; }

        public long? DgppId { get; set; }

        [Dependency(DependencyType.ReadOnly, "LegalPerson", "!this.value")]
        public long? ClientId { get; set; }

        public decimal PayablePrice { get; set; }

        public decimal PayablePlan { get; set; }

        public decimal PayableFact { get; set; }

        // Скрытое поле, заведено в контекте бага 1735
        public decimal VatPlan { get; set; }

        public decimal AmountToWithdraw { get; set; }

        public decimal AmountWithdrawn { get; set; }

        [Dependency(DependencyType.ReadOnly, "Bargain", "!Ext.getCmp('LegalPerson').getValue() || !Ext.getCmp('BranchOfficeOrganizationUnit').getValue()")]
        public LookupField Bargain { get; set; }

        [CustomClientValidation("validateDiscountSum", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositive")]
        [Dependency(DependencyType.Required, "DiscountReason", "(this.value && (this.value == this.value) && (+(this.value.replace(',', '.')) > 0))")]
        [Dependency(DependencyType.Required, "DiscountComment", "(this.value && (this.value == this.value) && (+(this.value.replace(',', '.')) > 0))")]
        public decimal? DiscountSum { get; set; }

        [CustomClientValidation("validateDiscountPercent", ErrorMessageResourceType = typeof(BLResources),
            ErrorMessageResourceName = "DiscountPercentMustBeBetweenZeroAndOneHundred")]
        [Dependency(DependencyType.Required, "DiscountReason", "(this.value && (this.value == this.value) && (+(this.value.replace(',', '.')) > 0))")]
        [Dependency(DependencyType.Required, "DiscountComment", "(this.value && (this.value == this.value) && (+(this.value.replace(',', '.')) > 0))")]
        public decimal? DiscountPercent { get; set; }

        public bool DiscountPercentChecked { get; set; }

        public OrderDiscountReason DiscountReason { get; set; }

        [StringLengthLocalized(300)]
        public string DiscountComment { get; set; }

        [StringLengthLocalized(300)]
        [DisplayNameLocalized("CancellationComment")]
        public string Comment { get; set; }

        public bool IsTerminated { get; set; }

        [Dependency(DependencyType.Required, "Comment", "this.value=='TemporaryRejectionOther' || this.value=='RejectionOther'")]
        public OrderTerminationReason TerminationReason { get; set; }

        [ExcludeZeroValue]
        public OrderType OrderType { get; set; }

        public LookupField Inspector { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public PaymentMethod PaymentMethod { get; set; }

        public override LookupField Owner { get; set; }

        public string Platform { get; set; }

        public long? PlatformId { get; set; }

        public bool IsWorkflowLocked { get; set; }

        public long CurrenctUserCode { get; set; }

        [Dependency(DependencyType.Required, "DocumentsComment", "this.value == 'Other'")]
        public DocumentsDebt HasDocumentsDebt { get; set; }

        [StringLengthLocalized(300)]
        public string DocumentsComment { get; set; }

        public long? AccountId { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public Uri OrderValidationServiceUrl { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OrderDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Number = modelDto.Number;
            RegionalNumber = modelDto.RegionalNumber;
            Firm = LookupField.FromReference(modelDto.FirmRef);
            ClientId = modelDto.ClientRef != null ? modelDto.ClientRef.Id : null;
            DgppId = modelDto.DgppId;
            HasAnyOrderPosition = modelDto.HasAnyOrderPosition;
            HasDestOrganizationUnitPublishedPrice = modelDto.HasDestOrganizationUnitPublishedPrice;
            SourceOrganizationUnit = LookupField.FromReference(modelDto.SourceOrganizationUnitRef);
            DestinationOrganizationUnit = LookupField.FromReference(modelDto.DestOrganizationUnitRef);
            BranchOfficeOrganizationUnit = LookupField.FromReference(modelDto.BranchOfficeOrganizationUnitRef);
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            Deal = LookupField.FromReference(modelDto.DealRef);
            DealCurrencyId = modelDto.DealCurrencyId;
            LegalPersonProfile = LookupField.FromReference(modelDto.LegalPersonProfileRef);
            Currency = LookupField.FromReference(modelDto.CurrencyRef);
            BeginDistributionDate = modelDto.BeginDistributionDate;
            EndDistributionDatePlan = modelDto.EndDistributionDatePlan;
            EndDistributionDateFact = modelDto.EndDistributionDateFact;
            BeginReleaseNumber = modelDto.BeginReleaseNumber;
            EndReleaseNumberPlan = modelDto.EndReleaseNumberPlan;
            EndReleaseNumberFact = modelDto.EndReleaseNumberFact;
            SignupDate = modelDto.SignupDate;
            ReleaseCountPlan = modelDto.ReleaseCountPlan;
            ReleaseCountFact = modelDto.ReleaseCountFact;
            PreviousWorkflowStepId = (int)modelDto.PreviousWorkflowStepId;
            WorkflowStepId = (int)modelDto.WorkflowStepId;
            PayablePlan = modelDto.PayablePlan;
            PayableFact = modelDto.PayableFact;
            PayablePrice = modelDto.PayablePrice;
            VatPlan = modelDto.VatPlan;
            AmountToWithdraw = modelDto.AmountToWithdraw;
            AmountWithdrawn = modelDto.AmountWithdrawn;
            DiscountSum = modelDto.DiscountSum;
            DiscountPercent = modelDto.DiscountPercent;
            DiscountReason = modelDto.DiscountReasonEnum;
            DiscountComment = modelDto.DiscountComment;
            DiscountPercentChecked = modelDto.DiscountPercentChecked;
            Comment = modelDto.Comment;
            IsTerminated = modelDto.IsTerminated;
            TerminationReason = modelDto.TerminationReason;
            OrderType = modelDto.OrderType;
            Inspector = LookupField.FromReference(modelDto.InspectorRef);
            Bargain = LookupField.FromReference(modelDto.BargainRef);
            Platform = modelDto.Platform;
            PlatformId = modelDto.PlatformRef != null ? modelDto.PlatformRef.Id : null;
            PaymentMethod = modelDto.PaymentMethod;
            HasDocumentsDebt = modelDto.HasDocumentsDebt;
            DocumentsComment = modelDto.DocumentsComment;
            AccountId = modelDto.AccountRef != null ? modelDto.AccountRef.Id : null;
            ShowRegionalAttributes = modelDto.ShowRegionalAttributes;

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new OrderDomainEntityDto
                          {
                              Id = Id,
                              Number = Number,
                              RegionalNumber = RegionalNumber,
                              FirmRef = Firm.ToReference(),
                              ClientRef = new EntityReference(ClientId),
                              DgppId = DgppId,
                              HasAnyOrderPosition = HasAnyOrderPosition,
                              HasDestOrganizationUnitPublishedPrice = HasDestOrganizationUnitPublishedPrice,
                              BranchOfficeOrganizationUnitRef = BranchOfficeOrganizationUnit.ToReference(),
                              LegalPersonRef = LegalPerson.ToReference(),
                              DealRef = Deal.ToReference(),
                              DealCurrencyId = DealCurrencyId,
                              LegalPersonProfileRef = LegalPersonProfile.ToReference(),
                              CurrencyRef = Currency.ToReference(),
                              BeginDistributionDate = BeginDistributionDate,
                              EndDistributionDatePlan = EndDistributionDatePlan,
                              EndDistributionDateFact = EndDistributionDateFact,
                              BeginReleaseNumber = BeginReleaseNumber,
                              EndReleaseNumberPlan = EndReleaseNumberPlan,
                              EndReleaseNumberFact = EndReleaseNumberFact,
                              SignupDate = SignupDate,
                              ReleaseCountPlan = ReleaseCountPlan,
                              ReleaseCountFact = ReleaseCountFact,
                              PreviousWorkflowStepId = (OrderState)PreviousWorkflowStepId,
                              WorkflowStepId = (OrderState)WorkflowStepId,
                              PayablePlan = PayablePlan,
                              PayableFact = PayableFact,
                              PayablePrice = PayablePrice,
                              VatPlan = VatPlan,
                              AmountToWithdraw = AmountToWithdraw,
                              AmountWithdrawn = AmountWithdrawn,
                              DiscountSum = DiscountSum,
                              DiscountPercent = DiscountPercent,
                              DiscountReasonEnum = DiscountReason,
                              DiscountComment = DiscountComment,
                              DiscountPercentChecked = DiscountPercentChecked,
                              Comment = Comment,
                              IsTerminated = IsTerminated,
                              TerminationReason = TerminationReason,
                              OrderType = OrderType,
                              InspectorRef = Inspector.ToReference(),
                              BargainRef = Bargain.ToReference(),
                              Platform = Platform,
                              PlatformRef = new EntityReference(PlatformId),
                              HasDocumentsDebt = HasDocumentsDebt,
                              DocumentsComment = DocumentsComment,
                              AccountRef = new EntityReference(AccountId),
                              PaymentMethod = PaymentMethod,
                              OwnerRef = Owner.ToReference(),
                              Timestamp = Timestamp,
                          };

            if (SourceOrganizationUnit.Key.HasValue)
            {
                dto.SourceOrganizationUnitRef = SourceOrganizationUnit.ToReference();
            }

            if (DestinationOrganizationUnit.Key.HasValue)
            {
                dto.DestOrganizationUnitRef = DestinationOrganizationUnit.ToReference();
            }

            return dto;
        }
    }
}