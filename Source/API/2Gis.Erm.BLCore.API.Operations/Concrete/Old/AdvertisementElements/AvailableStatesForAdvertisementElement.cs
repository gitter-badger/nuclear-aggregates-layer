using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementElements
{
    public sealed class AvailableStatesForAdvertisementElementRequest : Request
    {
        public AdvertisementElementStatus CurrentState { get; set; }
    }

    public sealed class AvailableStatesForAdvertisementElementResponse : Response
    {
        public AdvertisementElementStatus[] AvailableStates { get; set; }
    }
}