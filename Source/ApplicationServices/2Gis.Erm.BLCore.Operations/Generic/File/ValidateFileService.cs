using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public sealed class ValidateFileService : IValidateFileService
    {
        private readonly IValidateFileSettings _validateFileSettings;

        public ValidateFileService(IValidateFileSettings validateFileSettings)
        {
            _validateFileSettings = validateFileSettings;
        }

        public void Validate(UploadFileParams fileParams)
        {
            if (fileParams.ContentLength > _validateFileSettings.FileSizeLimit)
            {
                var message = string.Format(BLResources.FileTooLarge, _validateFileSettings.FileSizeLimit);
                throw new BusinessLogicException(message);
            }
        }
    }
}
