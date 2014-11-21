using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class FasCommentViewModel : ViewModel
    {
        public int? TemplateTextLengthRestriction { get; set; }
        public string FasCommentDisplayTextItemsJson { get; set; }

        [Dependency(DependencyType.Transfer, "PlainText", "var t = Ext.decode(Ext.getDom('FasComment_FasCommentDisplayTextItems').value); initially?undefined:t[this.value];")]
        [Dependency(DependencyType.ReadOnly, "PlainText", "this.value!='NewFasComment'")]
        public FasComment FasComment { get; set; }

        [DisplayNameLocalized("Text")]
        public string PlainText { get; set; }

        public void LoadDomainEntityDto(IFasCommentAdvertisementElementDomainEntityDto dto)
        {
            TemplateTextLengthRestriction = dto.TemplateTextLengthRestriction;

            PlainText = dto.PlainText;
            FasComment = dto.FasCommentType.Value;
        }

        public void TransferToDomainEntityDto(IFasCommentAdvertisementElementDomainEntityDto dto)
        {
            dto.PlainText = PlainText;
            dto.FasCommentType = FasComment;
        }
    }
}