using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    // TODO {all, 09.04.2014}: возможно, стоит унифицировать форму выбора профиля юрлица при печати заказа и счета
    public sealed class PrintOrderViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField LegalPersonProfile { get; set; }

        public PrintOrderType PrintOrderType { get; set; }

        public long? DefaultLegalPersonProfileId { get; set; }

        public long LegalPersonId { get; set; }

        public long OrderId { get; set; }
    }
}