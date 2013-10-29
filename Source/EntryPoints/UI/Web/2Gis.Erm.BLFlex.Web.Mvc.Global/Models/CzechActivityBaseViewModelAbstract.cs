using System;

using DoubleGis.Erm.Core.Enums;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;
using DoubleGis.Erm.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public abstract class CzechActivityBaseViewModelAbstract<T> : EntityViewModelBase<T>, ICzechAdapted where T : ActivityBase, new()
    {
        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        [RequiredLocalized, ExcludeZeroValue]
        [DisplayNameLocalized("ActivityBaseViewModelAbstract_Type")]
        [Dependency(DependencyType.Disable, "", "true")]
        [Dependency(DependencyType.NotRequiredDisableHide, "TaskType", "this.value != 'Task'")]
        public ActivityType Type { get; set; }

        [RequiredLocalized, ExcludeZeroValue]
        public ActivityPriority Priority { get; set; }

        [RequiredLocalized, ExcludeZeroValue]
        [DisplayNameLocalized("ActivityBaseViewModelAbstract_Status")]
        [Dependency(DependencyType.Hidden, "ActualEnd", "this.value == 'InProgress'")]
        [Dependency(DependencyType.Hidden, "ActualEndTime", "this.value == 'InProgress'")]
        public ActivityStatus Status { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("Title")]
        public string Header { get; set; }

        [RequiredLocalized]
        public DateTime ScheduledStart { get; set; }

        public TimeSpan ScheduledStartTime { get; set; }

        public string Duration { get; set; }

        [RequiredLocalized]
        public DateTime ScheduledEnd { get; set; }

        public DateTime? ActualEnd { get; set; }

        public TimeSpan? ActualEndTime { get; set; }

        public TimeSpan ScheduledEndTime { get; set; }

        public string Description { get; set; }

        public LookupField Client { get; set; }

        public LookupField Deal { get; set; }

        public LookupField Firm { get; set; }

        public LookupField Contact { get; set; }

        public override LookupField Owner { get; set; }

        public override byte[] Timestamp { get; set; }
    }
}