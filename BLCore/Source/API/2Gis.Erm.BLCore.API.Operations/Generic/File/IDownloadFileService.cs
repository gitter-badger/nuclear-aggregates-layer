using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IDownloadFileService : IOperation<DownloadIdentity>
    {
        StreamResponse DownloadFile(long fileId);
    }
}
