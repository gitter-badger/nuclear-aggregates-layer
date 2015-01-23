using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Common.Serialization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureOrderPositionViewModel : EntityViewModelBase<OrderPosition>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings() { Converters = { new Int64ToStringConverter() } };

        public long OrderId { get; set; }

        [RequiredLocalized]
        public LookupField PricePosition { get; set; }

        public decimal PricePerUnit { get; set; }

        [RequiredLocalized]
        [CustomClientValidation("validatePricePerUnitWithVat", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositive")]
        public decimal PricePerUnitWithVat { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("Quantity")]
        [CustomClientValidation("validateAmount", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositive")]
        public int Amount { get; set; }

        [RequiredLocalized]
        [CustomClientValidation("validateDiscountSum", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositive")]
        public decimal DiscountSum { get; set; }

        [CustomClientValidation("validateDiscountPercent", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "DiscountPercentMustBeBetweenZeroAndOneHundred")]
        [RequiredLocalized]
        public decimal DiscountPercent { get; set; }

        public decimal PayablePrice { get; set; }

        [CustomClientValidation("validatePayablePlan", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositive")]
        public decimal PayablePlan { get; set; }

        public int ShipmentPlan { get; set; }

        [StringLengthLocalized(300)]
        public string Comment { get; set; }

        public bool IsComposite { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public long PriceId { get; set; }
        public long? OrderFirmId { get; set; }
        public string OrderNumber { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public decimal CategoryRate { get; set; }

        public int MoneySignificantDigitsNumber { get; set; }

        public bool IsRated { get; set; }

        [Dependency(DependencyType.Disable, "CalculateDiscountViaPercentTrue", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.Disable, "CalculateDiscountViaPercentFalse", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "Comment", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "DiscountPercentText", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "DiscountSumText", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "DiscountPercent", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "DiscountSum", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "CalculateDiscountViaPercentTrue", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "CalculateDiscountViaPercentFalse", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "Amount", "!window.Ext.getDom('PricePosition').value || this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "PricePosition", "this.value && this.value.toLowerCase()=='true'")]
        [Dependency(DependencyType.ReadOnly, "PricePerUnitVat", "this.value && this.value.toLowerCase()=='true'")]
        public bool IsLocked { get; set; }

        public string AdvertisementsJson { get; set; }

        public bool IsBlockedByRelease { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OrderPositionDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            OrderId = modelDto.OrderId;
            OrderNumber = modelDto.OrderNumber;

            IsComposite = modelDto.IsComposite;
            OrganizationUnitId = modelDto.OrganizationUnitId;
            PeriodStartDate = modelDto.PeriodStartDate;
            PeriodEndDate = modelDto.PeriodEndDate;

            Amount = modelDto.Amount;
            PricePerUnit = modelDto.PricePerUnit;
            PricePerUnitWithVat = modelDto.PricePerUnitWithVat;
            DiscountPercent = modelDto.DiscountPercent;
            DiscountSum = modelDto.DiscountSum;
            PayablePrice = modelDto.PayablePrice;
            PayablePlan = modelDto.PayablePlan;
            ShipmentPlan = modelDto.ShipmentPlan;

            Comment = modelDto.Comment;
            CalculateDiscountViaPercent = modelDto.CalculateDiscountViaPercent;

            PricePosition = LookupField.FromReference(modelDto.PricePositionRef);

            AdvertisementsJson = JsonConvert.SerializeObject(modelDto.Advertisements, Settings);

            PriceId = modelDto.PriceId;
            OrderFirmId = modelDto.OrderFirmId;

            CategoryRate = modelDto.CategoryRate;

            if (modelDto.IsBlockedByRelease || modelDto.OrderWorkflowStepId == (int)OrderState.Archive)
            {
                ViewConfig.ReadOnly = true;
            }

            IsLocked = modelDto.OrderWorkflowStepId != (int)OrderState.OnRegistration;

            Owner = LookupField.FromReference(modelDto.OwnerRef);
            CreatedBy = LookupField.FromReference(modelDto.CreatedByRef);
            ModifiedBy = LookupField.FromReference(modelDto.ModifiedByRef);
            Timestamp = modelDto.Timestamp;
            IsDeleted = modelDto.IsDeleted;
            IsActive = modelDto.IsActive;
            CreatedOn = modelDto.CreatedOn;
            ModifiedOn = modelDto.ModifiedOn;
            IsRated = modelDto.IsRated;
            IsBlockedByRelease = modelDto.IsBlockedByRelease;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new OrderPositionDomainEntityDto
                {
                    Id = Id,
                    OrderId = OrderId,
                    OrderNumber = OrderNumber,

                    IsComposite = IsComposite,
                    OrganizationUnitId = OrganizationUnitId,
                    PeriodStartDate = PeriodStartDate,
                    PeriodEndDate = PeriodEndDate,

                    Amount = Amount,
                    PricePerUnit = PricePerUnit,
                    PricePerUnitWithVat = PricePerUnitWithVat,
                    DiscountPercent = DiscountPercent,
                    DiscountSum = DiscountSum,
                    PayablePrice = PayablePrice,
                    PayablePlan = PayablePlan,
                    ShipmentPlan = ShipmentPlan,

                    Comment = Comment,
                    CalculateDiscountViaPercent = CalculateDiscountViaPercent,

                    Advertisements = JsonConvert.DeserializeObject<AdvertisementDescriptor[]>(AdvertisementsJson),

                    PriceId = PriceId,
                    OrderFirmId = OrderFirmId,

                    Timestamp = Timestamp
                };

            if (PricePosition.Key.HasValue)
            {
                dto.PricePositionRef = PricePosition.ToReference();
            }

            return dto;
        }
    }
}