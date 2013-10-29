using System.IO;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    internal class DatabaseMigrationContext : IMigrationContext
    {
        private readonly string _connectionString;
        private readonly bool _isCaptureMode;
        private bool _isConnectionInitialized;

        private Database _database;
        private ServerConnection _connection;
        private MigrationOptions _options;

        public DatabaseMigrationContext(string connectionString, TextWriter output, bool isCaptureMode = false)
        {
            _connectionString = connectionString;
            _isCaptureMode = isCaptureMode;
            Output = output;
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

        public MigrationOptions Options
        {
            get
            {
                EnsureInitialized();
                return _options;
            }
        }

        public TextWriter Output { get; private set; }

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

                _options = new MigrationOptions(ConnectionStringsKnower.GetEnvironmentSuffix(_connectionString));
            }
        }
    }
}
