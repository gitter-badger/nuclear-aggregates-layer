using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class PlainTextViewModel : ViewModel
    {
        public int? TemplateTextLengthRestriction { get; set; }
        public byte? TemplateMaxSymbolsInWord { get; set; }
        public int? TemplateTextLineBreaksRestriction { get; set; }

        [DisplayNameLocalized("Text")]
        public string PlainText { get; set; }

        public void LoadDomainEntityDto(ITextAdvertisementElementDomainEntityDto dto)
        {
            TemplateTextLengthRestriction = dto.TemplateTextLengthRestriction;
            TemplateMaxSymbolsInWord = dto.TemplateMaxSymbolsInWord;
            TemplateTextLineBreaksRestriction = dto.TemplateTextLineBreaksRestriction;

            PlainText = dto.PlainText;
        }

        public void TransferToDomainEntityDto(ITextAdvertisementElementDomainEntityDto dto)
        {
            dto.PlainText = PlainText;
        }
    }
}