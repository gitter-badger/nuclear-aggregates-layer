using System.Configuration;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IStandartConfigurationSettings : ISettings
    {
        Configuration StandartConfiguration { get; }
    }
}