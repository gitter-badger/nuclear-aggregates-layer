using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CheckOrdersReadinessForReleaseDialogViewModel : ViewModel
    {
        [RequiredLocalized]
        // TODO : переименовать в Month
        public DateTime StartPeriodDate { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        public LookupField Owner { get; set; }

        public bool IncludeOwnerDescendants { get; set; }

        public bool CheckAccountBalance { get; set; }

        public bool? HasErrors { get; set; }
        public Guid ErrorLogFileId { get; set; }
    }
}