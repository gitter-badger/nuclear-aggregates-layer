using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Qds.Common.Settings;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Qds.Migrator
{
    internal sealed class SearchMigratorSettings : SettingsContainerBase
    {
        public SearchMigratorSettings()
        {
            var connectionStrings = new ConnectionStringsSettingsAspect();
            Aspects
               .Use(connectionStrings)
               .Use<EnvironmentsAspect>()
               .Use<DebtProcessingSettingsAspect>()
               .Use(new MsCRMSettingsAspect(connectionStrings)) // IFinder не сресолвится без ms crm settings
               .Use(new NestSettingsAspect(connectionStrings));
        }
    }
}