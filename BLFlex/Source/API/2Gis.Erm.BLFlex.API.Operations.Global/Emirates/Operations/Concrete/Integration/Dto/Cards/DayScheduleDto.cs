using System;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards
{
    // COMMENT {all, 26.05.2014}: В случае отказа от хранимки импорта карточек на всех инсталляциях, эта Dto'ха может переехать в BLCore
    public sealed class DayScheduleDto
    {
        public DayLabel Label { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
        public BreakDto Break { get; set; }
    }
}