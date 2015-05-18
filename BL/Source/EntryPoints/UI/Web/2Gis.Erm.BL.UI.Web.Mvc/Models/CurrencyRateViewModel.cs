using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CurrencyRateViewModel : EntityViewModelBase<CurrencyRate>
    {
        [RequiredLocalized]
        [RegularExpression(@"^([\d]((.|,)[\d]{1,})+|[1-9][\d]*((.|,)[\d]{1,})?)$", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositiveDigit")]
        public decimal Rate { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("ForCurrency")]
        public LookupField Currency { get; set; }

        [RequiredLocalized]
        public LookupField BaseCurrency { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (CurrencyRateDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            BaseCurrency = LookupField.FromReference(modelDto.BaseCurrencyRef);
            Currency = LookupField.FromReference(modelDto.CurrencyRef);
            Rate = modelDto.Rate;

            if (modelDto.IsCurrent)
            {
                SetInfo(BLResources.CurrencyRateViewModel_ChoosenRateIsAlreadyCurrent);
            }

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new CurrencyRateDomainEntityDto
                {
                    Id = Id,
                    BaseCurrencyRef = BaseCurrency.ToReference(),
                    CurrencyRef = Currency.ToReference(),
                    Rate = Rate,
                    Timestamp = Timestamp
                };
        }
    }
}