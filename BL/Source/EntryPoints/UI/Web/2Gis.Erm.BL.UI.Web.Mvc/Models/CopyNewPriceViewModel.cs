using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CopyNewPriceViewModel : ViewModel
    {
        private DateTime _publishDate;
        private DateTime _beginDate;

        [PresentationLayerProperty]
        public long Id { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PricePublishDate")]
        public DateTime PublishDate
        {
            get { return _publishDate; }
            set { _publishDate = value.AssumeUtcKind(); }
        }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PublishBeginDate")]
        [GreaterOrEqualThan("PublishDate", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "BeginDateMustBeGreaterOrEqualThenPublishDate")]
        [CheckDayOfMonth(CheckDayOfMonthType.FirstDay, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "RequiredFirstDayOfMonthMessage")]
        public DateTime BeginDate
        {
            get { return _beginDate; }
            set { _beginDate = value.AssumeUtcKind(); }
        }
    }
}