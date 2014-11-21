using System.IO;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IFileService : ISimplifiedModelConsumer, IInvariantSafeCrosscuttingService
    {
        FileWithContent GetFileById(long fileId);
        Stream GetFileContent(long fileId);
        void Add(FileWithContent file);
        void DeleteOrhpanFiles();
    }
}
