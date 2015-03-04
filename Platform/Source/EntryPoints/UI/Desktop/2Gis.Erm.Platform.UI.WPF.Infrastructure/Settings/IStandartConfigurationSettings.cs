using System.Configuration;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IStandartConfigurationSettings : ISettings
    {
        Configuration StandartConfiguration { get; }
    }
}