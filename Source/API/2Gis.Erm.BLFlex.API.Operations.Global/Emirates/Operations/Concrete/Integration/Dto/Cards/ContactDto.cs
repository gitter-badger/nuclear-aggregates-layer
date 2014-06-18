using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards
{ 
    // COMMENT {all, 26.05.2014}: В случае отказа от хранимки импорта карточек на всех инсталляциях, эта Dto'ха может переехать в BLCore
    public sealed class ContactDto
    {
        public ContactType ContactType { get; set; }
        public string Contact { get; set; }
        public int SortingPosition { get; set; }
        public int? FormatCode { get; set; }
        public long? PhoneZoneCode { get; set; }
    }
}