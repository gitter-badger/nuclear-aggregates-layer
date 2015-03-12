using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IValidateFileSettings : ISettings
    {
        int FileSizeLimit { get; }
    }
}