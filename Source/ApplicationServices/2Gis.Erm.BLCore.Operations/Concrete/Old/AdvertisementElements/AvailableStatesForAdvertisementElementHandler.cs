using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementElements;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.AdvertisementElements
{
    public class AvailableStatesForAdvertisementElementHandler : RequestHandler<AvailableStatesForAdvertisementElementRequest, AvailableStatesForAdvertisementElementResponse>
    {
        protected override AvailableStatesForAdvertisementElementResponse Handle(AvailableStatesForAdvertisementElementRequest request)
        {
            switch (request.CurrentState)
            {
                case AdvertisementElementStatus.NotValidated:
                    return new AvailableStatesForAdvertisementElementResponse
                    {
                        AvailableStates = new[] { AdvertisementElementStatus.NotValidated, AdvertisementElementStatus.Valid, AdvertisementElementStatus.Invalid }
                    };
                case AdvertisementElementStatus.Valid:
                case AdvertisementElementStatus.Invalid:
                    return new AvailableStatesForAdvertisementElementResponse
                    {
                        AvailableStates = new[] { AdvertisementElementStatus.Valid, AdvertisementElementStatus.Invalid }
                    };
                default:
                    throw new ArgumentOutOfRangeException("request");
            }
        }
    }
}