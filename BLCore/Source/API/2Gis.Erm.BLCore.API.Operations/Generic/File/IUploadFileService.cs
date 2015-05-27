using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IUploadFileService : IOperation<UploadIdentity>
    {
        UploadFileResult UploadFile(UploadFileParams uploadFileParams);
    }
}
