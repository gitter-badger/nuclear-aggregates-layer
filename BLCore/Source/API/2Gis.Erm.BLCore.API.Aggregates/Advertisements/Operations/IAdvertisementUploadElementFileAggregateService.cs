using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations
{
    public interface IAdvertisementUploadElementFileAggregateService : IAggregateSpecificOperation<Advertisement, UploadIdentity>
    {
        UploadFileResult UploadFile(AdvertisementElement advertisementElement,
                                    UploadFileParams<AdvertisementElement> uploadFileParams);
    }
}