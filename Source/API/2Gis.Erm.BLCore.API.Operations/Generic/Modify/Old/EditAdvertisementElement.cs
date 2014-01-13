using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementElementRequest : EditRequest<AdvertisementElement>
    {
        public string FileTimestamp { get; set; }

        public string PlainText { get; set; }
        public string FormattedText { get; set; }
    }
}
