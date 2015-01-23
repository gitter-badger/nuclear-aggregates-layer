using System;
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
    public sealed class AdsTemplatesAdsElementTemplateViewModel : EntityViewModelBase<AdsTemplatesAdsElementTemplate>
    {
        [RequiredLocalized]
        [DisplayNameLocalized("AdvertisementTemplateName")]
        public LookupField AdvertisementTemplate { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("AdvertisementElementTemplateName")]
        public LookupField AdvertisementElementTemplate { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "ExportCodeRangeMessage")]
        [RequiredLocalized]
        public int? ExportCode { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdsTemplatesAdsElementTemplateDomainEntityDto)domainEntityDto;

            Id = dto.Id;
            AdvertisementTemplate = LookupField.FromReference(dto.AdsTemplateRef);
            AdvertisementElementTemplate = LookupField.FromReference(dto.AdsElementTemplateRef);
            ExportCode = dto.ExportCode;
            Timestamp = dto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (ExportCode == null)
            {
                throw new ArgumentNullException();
            }

            return new AdsTemplatesAdsElementTemplateDomainEntityDto
                {
                    Id = Id,
                    AdsTemplateRef = AdvertisementTemplate.ToReference(),
                    AdsElementTemplateRef = AdvertisementElementTemplate.ToReference(),
                    ExportCode = ExportCode.Value,
                    Timestamp = Timestamp
                };
        }
    }
}