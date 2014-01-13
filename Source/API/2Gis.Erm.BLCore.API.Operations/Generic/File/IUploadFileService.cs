using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IUploadFileService : IOperation<UploadIdentity>
    {
        UploadFileResult UploadFile(UploadFileParams uploadFileParams);
    }
}
