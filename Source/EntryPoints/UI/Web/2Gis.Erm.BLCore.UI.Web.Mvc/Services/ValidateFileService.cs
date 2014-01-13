using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services
{
    public sealed class ValidateFileService : IValidateFileService
    {
        private readonly IWebAppSettings _appSettings;

        public ValidateFileService(IWebAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Validate(UploadFileParams fileParams)
        {
            if (fileParams.ContentLength > _appSettings.FileSizeLimit)
            {
                var message = string.Format(BLResources.FileTooLarge, _appSettings.FileSizeLimit);
                throw new BusinessLogicException(message);
            }
        }
    }
}
