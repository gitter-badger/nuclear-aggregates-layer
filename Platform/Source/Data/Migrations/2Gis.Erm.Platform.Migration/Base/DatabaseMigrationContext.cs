using System.IO;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class DatabaseMigrationContext : IMigrationContext
    {
        private readonly string _connectionString;
        private readonly bool _isCaptureMode;
        private bool _isConnectionInitialized;

        private Database _database;
        private ServerConnection _connection;

        public DatabaseMigrationContext(
            string connectionString, 
            string crmDatabaseName, 
            string loggingDatabaseName, 
            string ermDatabaseName, 
            TextWriter output, 
            bool isCaptureMode = false)
        {
            _connectionString = connectionString;
            _isCaptureMode = isCaptureMode;
            Output = output;
            CrmDatabaseName = crmDatabaseName;
            LoggingDatabaseName = loggingDatabaseName;
            ErmDatabaseName = ermDatabaseName;
        }

        public Database Database
        {
            get
            {
                EnsureInitialized();
                return _database;
            }
        }

        public ServerConnection Connection
        {
            get
            {
                EnsureInitialized();
                return _connection;
            }
        }

        public TextWriter Output { get; private set; }

        public string ErmDatabaseName { get; private set; }
        public string LoggingDatabaseName { get; private set; }
        public string CrmDatabaseName { get; private set; }

        public void Dispose()
        {
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Disconnect();
            }
        }

        private void EnsureInitialized()
        {
            if (!_isConnectionInitialized)
            {
                _isConnectionInitialized = true;

                var server = new Server();
                server.ConnectionContext.ConnectionString = _connectionString;
                server.ConnectionContext.SqlExecutionModes = _isCaptureMode 
                    ? SqlExecutionModes.CaptureSql
                    : SqlExecutionModes.ExecuteSql;

                if (!server.ConnectionContext.IsOpen)
                {
                    server.ConnectionContext.Connect();
                }

                _database = server.Databases[server.ConnectionContext.SqlConnectionObject.Database];
                _connection = server.ConnectionContext;
            }
        }
    }
}
