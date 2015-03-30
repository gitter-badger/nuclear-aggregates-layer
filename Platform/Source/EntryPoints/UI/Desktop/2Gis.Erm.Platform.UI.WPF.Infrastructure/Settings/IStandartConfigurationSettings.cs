using System.Configuration;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IStandartConfigurationSettings : ISettings
    {
        Configuration StandartConfiguration { get; }
    }
}