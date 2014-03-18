using System.Configuration;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IStandartConfigurationSettings : ISettings
    {
        Configuration StandartConfiguration { get; }
    }
}