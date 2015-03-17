using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class SpecifyOrderDocumentsDebtViewModel : ViewModel
    {
        public LookupField Order { get; set; }

        public DocumentsDebt HasDocumentsDebt { get; set; }

        [StringLengthLocalized(300)]
        public string DocumentsComment { get; set; }
    }
}