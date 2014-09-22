using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class MakeRegionalAdsDocsViewModel : ViewModel
    {
        [Calendar]
        [RequiredLocalized]
        public DateTime StartPeriodDate { get; set; }

        [RequiredLocalized]
        public LookupField SourceOrganizationUnit { get; set; }

        public long UserId { get; set; }
        public Guid? ResultOperationGuid { get; set; }
    }
}