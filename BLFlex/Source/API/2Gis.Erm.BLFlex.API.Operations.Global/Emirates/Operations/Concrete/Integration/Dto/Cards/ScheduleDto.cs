namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards
{
    // COMMENT {all, 26.05.2014}: В случае отказа от хранимки импорта карточек на всех инсталляциях, эта Dto'ха может переехать в BLCore
    public sealed class ScheduleDto
    {
        public ScheduleDto()
        {
            Schedule = new DayScheduleDto[7];
        }

        public DayScheduleDto[] Schedule { get; private set; }
    }
}