using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.Migrator
{
    // TODO: слить вместе с основными миграциями
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Search Migrator");

            var container = new UnityContainer().ConfigureUnity(new SearchMigratorSettings());

            var migrationDescriptorsProvider = new AssemblyMigrationDescriptorsProvider(new[] { "2Gis.Erm.Qds.Migrations" });
            var migrationsProvider = new AssemblyMigrationsProvider();

            var versionsManager = container.Resolve<ElasticAppliedVersionsManager>();
            var context = container.Resolve<ElasticSearchMigrationContext>();

            var pendingMigrations = GetPendingMigrations(versionsManager, migrationDescriptorsProvider);
            foreach (var descriptor in pendingMigrations)
            {
                Console.WriteLine("Migrating: {0} ({1}), {2}", descriptor.Version, descriptor.Type.Name, descriptor.Description);

                var migration = (IContextedMigration<IElasticSearchMigrationContext>)migrationsProvider.GetMigrationImplementation(descriptor);

                migration.Apply(context);
                versionsManager.SaveVersionInfo(descriptor.Version);

                Console.WriteLine("Migrated: {0} ({1})\n", descriptor.Version, descriptor.Type.Name);
            }

            Console.WriteLine("Done");
        }

        private static IEnumerable<MigrationDescriptor> GetPendingMigrations(IAppliedVersionsReader appliedVersionsManager,
                                                                             IMigrationDescriptorsProvider migrationDescriptorsProvider)
        {
            appliedVersionsManager.LoadVersionInfo();
            var alreadyAppliedMigrations = appliedVersionsManager.AppliedVersionsInfo.GetAppliedMigrations();

            return migrationDescriptorsProvider.MigrationDescriptors
                                               .Where(x => !alreadyAppliedMigrations.Contains(x.Version))
                                               .Where(x => typeof(IContextedMigration<IElasticSearchMigrationContext>).IsAssignableFrom(x.Type))
                                               .OrderBy(x => x.Version)
                                               .ToArray();
        }
    }
}