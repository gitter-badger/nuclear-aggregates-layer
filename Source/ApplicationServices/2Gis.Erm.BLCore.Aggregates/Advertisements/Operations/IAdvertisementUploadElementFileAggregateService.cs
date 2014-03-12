using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public interface IAdvertisementUploadElementFileAggregateService : IAggregateSpecificOperation<Advertisement, UploadIdentity>
    {
        UploadFileResult UploadFile(AdvertisementElement advertisementElement, UploadFileParams<AdvertisementElement> uploadFileParams);
    }
}