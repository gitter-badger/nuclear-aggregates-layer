using System.IO;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class MigrationContextManager
    {
        private readonly string _connectionString;
        private readonly TextWriter _output;
        private readonly bool _isCaptureMode;

        public MigrationContextManager(string connectionString, TextWriter output, bool isCaptureMode = false)
        {
            _connectionString = connectionString;
            _output = output;
            _isCaptureMode = isCaptureMode;
        }

        public IMigrationContext GetContext()
        {
            return new DatabaseMigrationContext(_connectionString, _output, _isCaptureMode);
        }
    }
}
