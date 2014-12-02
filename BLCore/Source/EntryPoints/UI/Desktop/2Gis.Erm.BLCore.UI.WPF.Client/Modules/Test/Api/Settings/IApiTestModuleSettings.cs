using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Api.Settings
{
    public interface IApiTestModuleSettings : ISettings
    {
        int TestPropertyValue { get; }
    }
}
