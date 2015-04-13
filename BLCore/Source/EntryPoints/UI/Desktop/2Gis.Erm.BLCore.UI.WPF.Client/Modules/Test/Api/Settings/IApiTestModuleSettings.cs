using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Api.Settings
{
    public interface IApiTestModuleSettings : ISettings
    {
        int TestPropertyValue { get; }
    }
}
