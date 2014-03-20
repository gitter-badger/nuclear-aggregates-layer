using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Qds.API.Core.Settings;

namespace DoubleGis.Erm.Qds.Migrator.DI
{
    // TODO: удалить после того как SearchSettings перестанут наследоваться от CommonConfigFileAppSettings
    internal sealed class FakeAppSettings : SettingsContainerBase
    {
        public FakeAppSettings()
        {
            var connectionStrings = new ConnectionStringsSettingsAspect();
            Aspects
               .Use(connectionStrings)
               .Use<EnvironmentsAspect>()
               // IFinder не сресолвится без ms crm settings
               .Use(new MsCRMSettingsAspect(connectionStrings))
               .Use(new SearchSettingsAspect(connectionStrings));
        }
    }
}