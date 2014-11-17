using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    // TODO {s.pomadin, 26.08.2014}: Remove as it's a hack solution before UI changes
    public enum ActivityType
    {
        Appointment = 1,
        Phonecall = 2,
        Task = 3,
    }

    public abstract class ActivityBaseViewModelAbstract<T> : EntityViewModelBase<T>, ICustomizableActivityViewModel
		where T : IEntityKey
    {
	    protected ActivityBaseViewModelAbstract(ActivityType type)
    {
		    Type = type;
	    }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        [RequiredLocalized]
        [ExcludeZeroValue]
        [DisplayNameLocalized("ActivityBaseViewModelAbstract_Type")]
        [Dependency(DependencyType.Disable, "", "true")]
        public ActivityType Type { get; private set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public ActivityPriority Priority { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
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