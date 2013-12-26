using System.IO;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    using System;

    public class MigrationContextManager
    {
        private readonly ConnectionStringsKnower _connectionStringsKnower;
        private readonly TextWriter _output;
        private readonly bool _isCaptureMode;

        public MigrationContextManager(ConnectionStringsKnower connectionStringsKnower, TextWriter output, bool isCaptureMode = false)
        {
            _connectionStringsKnower = connectionStringsKnower;
            _output = output;
            _isCaptureMode = isCaptureMode;
        }

        public IMigrationContext GetContext(ErmConnectionStringKey key)
        {
            string connectionString;
            if (!_connectionStringsKnower.TryGetConnectionString(key, out connectionString))
            {
                // молча не применяем миграции
                if (key == ErmConnectionStringKey.CrmDatabase)
                {
                    return null;
                }

                throw new ArgumentException("Connection string not found");
            }

            string crmDatabaseName;
            if (!_connectionStringsKnower.TryGetDatabaseName(ErmConnectionStringKey.CrmConnection, out crmDatabaseName))
            {
                crmDatabaseName = null;
            }

            string loggingDatabaseName;
            if (!_connectionStringsKnower.TryGetDatabaseName(ErmConnectionStringKey.Logging, out loggingDatabaseName))
            {
                loggingDatabaseName = null;
            }

            string ermDatabaseName;
            if (!_connectionStringsKnower.TryGetDatabaseName(ErmConnectionStringKey.Erm, out ermDatabaseName))
            {
                ermDatabaseName = null;
            }

            return new DatabaseMigrationContext(connectionString, crmDatabaseName, loggingDatabaseName, ermDatabaseName, _output, _isCaptureMode);
        }
    }
}
