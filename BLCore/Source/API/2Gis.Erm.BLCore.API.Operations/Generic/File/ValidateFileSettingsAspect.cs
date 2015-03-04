using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public sealed class ValidateFileSettingsAspect : ISettingsAspect, IValidateFileSettings
    {
        private readonly IntSetting _fileSizeLimit = ConfigFileSetting.Int.Required("FileSizeLimit");

        public int FileSizeLimit
        {
            get { return _fileSizeLimit.Value; }
        }
    }
}