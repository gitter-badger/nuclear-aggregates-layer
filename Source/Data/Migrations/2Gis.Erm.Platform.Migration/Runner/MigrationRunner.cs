using System;
using System.Collections.Generic;
using System.IO;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Data.Services;
using Microsoft.Xrm.Client.Services;

namespace DoubleGis.Erm.Platform.Migration.Runner
{
    public class MigrationRunner : IDisposable
    {
        private readonly MigrationRunResult _runResult = new MigrationRunResult();
        private readonly TextWriter _output;
        private readonly bool _isCaptureMode;
        private readonly ConnectionStringsKnower _connectionStringsKnower;
        private readonly SmoAppliedVersionsManager _appliedVersionsManager;
        private readonly bool _useCrmConnection;
        private readonly IMigrationsProvider _migrationsProvider;

        public MigrationRunner(TextWriter output, bool isCaptureMode, ConnectionStringsKnower connectionStringsKnower, SmoAppliedVersionsManager appliedVersionsManager, bool useCrmConnection)
        {
            _output = output;
            _isCaptureMode = isCaptureMode;
            _connectionStringsKnower = connectionStringsKnower;
            _appliedVersionsManager = appliedVersionsManager;
            _useCrmConnection = useCrmConnection;
            _migrationsProvider = new AssemblyMigrationsProvider();
        }
        
        public void Dispose()
        {
            if (!_runResult.IsEmpty())
            {
                _output.WriteLine();
                if (_runResult.SuccessfullMigrations.Count > 0)
                {
                    _output.WriteLine("Migrations applied: {0}", string.Join(", ", _runResult.SuccessfullMigrations));
                }

                if (_runResult.FailureMigration.HasValue)
                {
                    _output.WriteLine("Migration failed: " + _runResult.FailureMigration);
                }

                _output.WriteLine();
            }
        }

        public void ApplyMigration(MigrationDescriptor m)
        {
            if (!_appliedVersionsManager.AppliedVersionsInfo.HasAppliedMigration(m.Version))
            {
                var migration = _migrationsProvider.GetMigrationImplementation(m);

                Log(string.Format("Migrating: {0} ({1}), {2}", m.Version, m.Type.Name, m.Description));

                try
                {
                    // Если нет подключения к базе Crm (например, на машинах разработчиков), то отмечаем миграцию как примененную, но не применяем её %|
                    if (!_useCrmConnection && migration is INonDefaultDatabaseMigration
                        && ((migration as INonDefaultDatabaseMigration).ConnectionStringKey == ErmConnectionStringKey.Crm ||
                            (migration as INonDefaultDatabaseMigration).ConnectionStringKey == ErmConnectionStringKey.CrmWebService))
                    {
                    }
                    else if (migration is IContextedMigration<IMigrationContext>)
                    {
                        var databaseMigration = migration as IContextedMigration<IMigrationContext>;
                        var contextManager = GetContextManager(databaseMigration);
                        using (var databaseContext = contextManager.GetContext())
                        {
                            databaseMigration.Apply(databaseContext);

                            if (_isCaptureMode)
                            {
                                _output.WriteLine("SQL to execute: ");
                                var capturedSql = databaseContext.Connection.CapturedSql.Text;

                                foreach (var s in capturedSql)
                                {
                                    _output.WriteLine(s);
                                    _output.WriteLine("GO");
                                    _output.WriteLine();
                                }
                            }
                        }
                    }
                    else if (migration is IContextedMigration<ICrmMigrationContext>)
                    {
                        var crmMigration = migration as IContextedMigration<ICrmMigrationContext>;
                        var crmContext = GetCrmContext();
                        crmMigration.Apply(crmContext);
                    }

                    _appliedVersionsManager.SaveVersionInfo(m.Version);
                    Log(string.Format("Migrated: {0} ({1})\n", m.Version, m.Type.Name));
                    _runResult.SuccessfullMigrations.Add(m.Version);
                }
                catch (MigrationKnownException ex)
                {
                    _runResult.FailureMigration = m.Version;
                    Log(ex.Message + "\n" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
                    throw;
                }
                catch (Exception ex)
                {
                    _runResult.FailureMigration = m.Version;
                    Log("Error during apply of the migration" + m.Version + ".\n" + ex);
                    throw;
                }
            }
        }

        public void RevertMigration(MigrationDescriptor m)
        {
            try
            {
                var migration = _migrationsProvider.GetMigrationImplementation(m);

                Log(string.Format("Reverting: {0} ({1}), {2}", m.Version, m.Type.Name, m.Description));

                try
                {
                    if (migration is IContextedMigration<IMigrationContext>)
                    {
                        var databaseMigration = migration as IContextedMigration<IMigrationContext>;
                        var contextManager = GetContextManager(databaseMigration);
                        using (var databaseContext = contextManager.GetContext())
                        {
                            databaseMigration.Revert(databaseContext);
                        }
                    }
                    else if (migration is IContextedMigration<ICrmMigrationContext>)
                    {
                        var crmMigration = migration as IContextedMigration<ICrmMigrationContext>;
                        var crmContext = GetCrmContext();
                        crmMigration.Revert(crmContext);
                    }

                    Log(string.Format("Reverted: {0} ({1})", m.Version, m.Type.Name));
                    _appliedVersionsManager.DeleteVersion(m.Version);
                    _runResult.SuccessfullMigrations.Add(m.Version);
                }
                catch (Exception ex)
                {
                    _runResult.FailureMigration = m.Version;
                    Log("Error when reverting the migration" + m.Version + ".\n" + ex);
                    throw;
                }
            }
            catch (KeyNotFoundException ex)
            {
                var msg = string.Format("AppliedVersionsInfo references version {0} but no Migrator was found attributed with that version.", m.Version);
                throw new Exception(msg, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error rolling back version " + m.Version, ex);
            }
        }

        protected virtual void Log(string message)
        {
            Console.WriteLine(message);
        }

        private MigrationContextManager GetContextManager(IContextedMigration<IMigrationContext> migration)
        {
            if (migration is INonDefaultDatabaseMigration)
            {
                var connectionStringKey = (migration as INonDefaultDatabaseMigration).ConnectionStringKey;
                return GetContextManager(connectionStringKey);
            }

            return GetContextManager();
        }

        private MigrationContextManager GetContextManager(ErmConnectionStringKey connectionStringKey = ErmConnectionStringKey.Default)
        {
            return new MigrationContextManager(_connectionStringsKnower.GetConnectionString(connectionStringKey), _output, _isCaptureMode);
        }

        private ICrmMigrationContext GetCrmContext()
        {
            var connectionString = _connectionStringsKnower.GetConnectionString(ErmConnectionStringKey.CrmWebService);
            var ermConnectionString = _connectionStringsKnower.GetConnectionString(ErmConnectionStringKey.Erm);

            var crmConnection = CrmConnection.Parse(connectionString);
            crmConnection.Timeout = 10 * 60 * 1000;
            var crmDataContext = new CrmDataContext(null, () => new OrganizationService(null, crmConnection));

            var migrationOptions = new MigrationOptions(ConnectionStringsKnower.GetEnvironmentSuffix(ermConnectionString));

            return new CrmMigrationContext(crmDataContext, migrationOptions);
        }
    }
}