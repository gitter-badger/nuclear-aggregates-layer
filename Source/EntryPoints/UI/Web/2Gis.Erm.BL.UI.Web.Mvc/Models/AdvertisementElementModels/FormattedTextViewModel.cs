using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class FormattedTextViewModel : ViewModel
    {
        public int? TemplateTextLengthRestriction { get; set; }
        public byte? TemplateMaxSymbolsInWord { get; set; }
        public int? TemplateTextLineBreaksRestriction { get; set; }

        public string PlainText { get; set; }

        [DisplayNameLocalized("Text")]
        public string FormattedText { get; set; }

        public void LoadDomainEntityDto(ITextAdvertisementElementDomainEntityDto dto)
        {
            PlainText = dto.PlainText;
            FormattedText = dto.FormattedText;

            TemplateTextLengthRestriction = dto.TemplateTextLengthRestriction;
            TemplateMaxSymbolsInWord = dto.TemplateMaxSymbolsInWord;
            TemplateTextLineBreaksRestriction = dto.TemplateTextLineBreaksRestriction;
        }

        public void TransferToDomainEntityDto(ITextAdvertisementElementDomainEntityDto dto)
        {
            dto.PlainText = PlainText;
            dto.FormattedText = Unescape(FormattedText);
        }

        private static string Unescape(string formattedText)
        {
            // делаем unescape на форматированный текст, т.к. до этого в js на него делаем escape
            // а escape в js мы сделали чтобы не передавать html в теле запроса, asp.net mvc ругается что это небезопасно
            return string.IsNullOrEmpty(formattedText)
                ? formattedText
                : Uri.UnescapeDataString(formattedText).Replace("\u001d", string.Empty).Replace("&nbsp;", " ");
        }
    }
}