﻿using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CurrencyViewModel : EditableIdEntityViewModelBase<Currency>, ICurrencyViewModel, INameAspect
    {
        [RequiredLocalized]
        [DisplayNameLocalized("CurrencyIsoCode")]
        public short IsoCode { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("CurrencySymbol")]
        [StringLengthLocalized(10)]
        public string Symbol { get; set; }

        [DisplayNameLocalized("IsBaseCurrency")]
        public bool IsBase { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (CurrencyDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            IsoCode = modelDto.ISOCode;
            Name = modelDto.Name;
            Symbol = modelDto.Symbol;
            IsBase = modelDto.IsBase;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new CurrencyDomainEntityDto
                {
                    Id = Id,
                    ISOCode = IsoCode,
                    Name = Name,
                    Symbol = Symbol,
                    IsBase = IsBase,
                    Timestamp = Timestamp
                };
        }
    }
}
