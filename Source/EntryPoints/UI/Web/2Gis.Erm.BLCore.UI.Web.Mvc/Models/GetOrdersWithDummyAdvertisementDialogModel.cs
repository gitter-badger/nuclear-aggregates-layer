using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class GetOrdersWithDummyAdvertisementDialogModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [RequiredLocalized]
        public LookupField Owner { get; set; }

        public bool IncludeOwnerDescendants { get; set; }

        public long UserId { get; set; }

        public bool? HasOrders { get; set; }
        public Guid OrdersListFileId { get; set; }
    }
}