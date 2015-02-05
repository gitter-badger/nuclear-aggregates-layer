using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CountryViewModel : EditableIdEntityViewModelBase<Country>
    {
        [RequiredLocalized]
        [StringLengthLocalized(150)]
        public string Name { get; set; }

        [RequiredLocalized]
        public LookupField Currency { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("CountryIsoCode")]
        [StringLengthLocalized(10)]
        public string IsoCode { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (CountryDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            Name = modelDto.Name;
            IsoCode = modelDto.IsoCode;
            Currency = LookupField.FromReference(modelDto.CurrencyRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new CountryDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    IsoCode = IsoCode,
                    CurrencyRef = Currency.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}