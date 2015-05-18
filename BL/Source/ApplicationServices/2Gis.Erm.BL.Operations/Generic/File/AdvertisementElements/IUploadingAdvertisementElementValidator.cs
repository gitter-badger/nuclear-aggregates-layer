using System.IO;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Generic.File.AdvertisementElements
{
    public interface IUploadingAdvertisementElementValidator : IInvariantSafeCrosscuttingService
    {
        void Validate(AdvertisementElementTemplate uploadingElementTemplate, string uploadingFileName, Stream uploadingFileContent);
    }
}
