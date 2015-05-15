using System;

using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class DatePropertyFeature : IPropertyFeature
    {
        public DatePropertyFeature()
        {
            ShiftOffset = true;
        }

        public DatePropertyFeature(bool shiftOffset, DateTime? minDate = null)
        {
            ShiftOffset = shiftOffset;
            MinDate = minDate;
        }

        public DatePropertyFeature(bool shiftOffset, PeriodType periodType, DisplayStyle displayStyle, DateTime? minDate = null)
        {
            ShiftOffset = shiftOffset;
            PeriodType = periodType;
            DisplayStyle = displayStyle;
            MinDate = minDate;
        }

        public bool ShiftOffset
        {
            get; 
            set;
        }

        public DisplayStyle DisplayStyle
        {
            get;
            private set;
        }

        public DateTime? MinDate
        {
            get;
            private set;
        }

        public DateTime? MaxDate
        {
            get;
            private set;
        }

        public PeriodType PeriodType
        {
            get;
            private set;
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
