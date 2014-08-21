using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementElementValidationState
    {
        public bool NeedsValidation { get; set; }
        public AdvertisementElementStatus CurrentStatus { get; set; }
    }
}