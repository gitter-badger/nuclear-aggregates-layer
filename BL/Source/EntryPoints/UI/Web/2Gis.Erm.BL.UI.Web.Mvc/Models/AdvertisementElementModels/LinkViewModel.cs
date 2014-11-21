using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class LinkViewModel : ViewModel
    {
        public int? TemplateTextLengthRestriction { get; set; }

        [UrlLocalized]
        [DisplayNameLocalized("Text")]
        public string PlainText { get; set; }

        public void LoadDomainEntityDto(ILinkAdvertisementElementDomainEntityDto dto)
        {
            PlainText = dto.PlainText;
            TemplateTextLengthRestriction = dto.TemplateTextLengthRestriction;
        }

        public void TransferToDomainEntityDto(ILinkAdvertisementElementDomainEntityDto dto)
        {
            dto.PlainText = PlainText;
        }
    }
}