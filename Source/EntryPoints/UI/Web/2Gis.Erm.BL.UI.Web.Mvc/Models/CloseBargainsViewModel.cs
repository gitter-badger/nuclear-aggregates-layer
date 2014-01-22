using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class CloseBargainsViewModel : ViewModel
    {
        public string BargainIds { get; set; }

        [RequiredLocalized]
        public DateTime? CloseDate { get; set; }

        public bool HasCompleted { get; set; }
    }
}