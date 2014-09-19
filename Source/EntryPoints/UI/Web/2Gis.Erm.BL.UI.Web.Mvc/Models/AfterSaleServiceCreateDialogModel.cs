using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AfterSaleServiceCreateDialogModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        public DateTime Month { get; set; }

        public long UserId { get; set; }
    }
}