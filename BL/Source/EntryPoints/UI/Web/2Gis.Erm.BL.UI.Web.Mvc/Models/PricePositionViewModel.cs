using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PricePositionViewModel : EntityViewModelBase<PricePosition>, IPriceAspect, IPositionAspect
    {
        public string PositionName
        {
            get { return Position.Value; }
        }

        public long PositionId
        {
            get { return Position.Key.Value; }
        }

        public string PriceName
        {
            get { return Price.Value; }
        }

        public long PriceId
        {
            get { return Price.Key.Value; }
        }

        [RequiredLocalized]
        public LookupField Position { get; set; }

        [RequiredLocalized]
        public decimal Cost { get; set; }

        public LookupField Currency { get; set; }

        [DisplayNameLocalized("PriceList")]
        public LookupField Price { get; set; }

        [DisplayNameLocalized("Quantity")]
        public int? Amount { get; set; }

        [DisplayNameLocalized("Quantity")]
        [Dependency(DependencyType.Required, "Amount", "this.value == 'FixedValue'")]
        public PricePositionAmountSpecificationMode AmountSpecificationMode { get; set; }

        public int? MinAdvertisementAmount { get; set; }

        public int? MaxAdvertisementAmount { get; set; }

        public PricePositionRateType RateType { get; set; }

        [Dependency(DependencyType.Hidden, "RateType", "this.value == 'False'")]
        public bool IsRateTypeAvailable { get; set; }

        [Dependency(DependencyType.DisableAndHide, "MaxAdvertisementAmount", "this.value == 'False'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "MinAdvertisementAmount", "this.value == 'False'")]
        public bool IsPositionControlledByAmount { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PricePositionDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;

            Price = LookupField.FromReference(modelDto.PriceRef);
            Position = LookupField.FromReference(modelDto.PositionRef);

            Cost = modelDto.Cost;
            Amount = modelDto.Amount;
            AmountSpecificationMode = modelDto.AmountSpecificationMode;
            RateType = modelDto.RateType;
            IsRateTypeAvailable = modelDto.IsRateTypeAvailable;
            IsPositionControlledByAmount = modelDto.IsPositionControlledByAmount;
            MinAdvertisementAmount = modelDto.MinAdvertisementAmount;
            MaxAdvertisementAmount = modelDto.MaxAdvertisementAmount;

            Currency = new LookupField { Key = modelDto.CurrencyRef.Id, Value = modelDto.CurrencyRef.Name };

            if (modelDto.PriceIsDeleted)
            {
                SetInfo(BLResources.CantEditPricePositionInDeactivatedPrice);
                ViewConfig.ReadOnly = true;
            }
            else if (modelDto.PriceIsPublished)
            {
                SetInfo(BLResources.CantEditPricePositionInPublishedPrice);
                ViewConfig.ReadOnly = true;
            }

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new PricePositionDomainEntityDto
            {
                Id = Id,
                PriceRef = Price.ToReference(),
                PositionRef = Position.ToReference(),
                Cost = Cost,
                Amount = Amount,
                AmountSpecificationMode = AmountSpecificationMode,
                RateType = RateType,
                IsRateTypeAvailable = IsRateTypeAvailable,
                MinAdvertisementAmount = MinAdvertisementAmount,
                MaxAdvertisementAmount = MaxAdvertisementAmount,
                Timestamp = Timestamp
            };
        }
    }
}

