using System;

using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    [Obsolete("������������ �� ������ ��������� ��������� - � ����� ������������� CalendarSettings")]
    public sealed class DateTimeSettings
    {
        private bool _shiftOffset = true;
        public bool ShowToday = true;

        public bool ShiftOffset
        {
            get { return _shiftOffset; }
            set { _shiftOffset = value; }
        }

        /// <summary>
        /// </summary>
        /// <remarks>��� �����, ���������� ��� Disabled, �� ����� � ���������� �� ������������ ��������.</remarks>
        public bool Disabled { get; set; }
        public bool ReadOnly { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public PeriodType PeriodType { get; set; }
        public DisplayStyle DisplayStyle { get; set; }
    }
}