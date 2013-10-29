using DoubleGis.Erm.Platform.Migration.Runner;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class MigrationOptions
    {
        public MigrationOptions(string environmentSuffix)
        {
            EnvironmentSuffix = environmentSuffix;
        }

        public string EnvironmentSuffix { get; private set; }

        public string GetMsCrmDatabaseName()
        {
            return EnvironmentUtil.GetMsCrmDatabaseName(EnvironmentSuffix);
        }
    }
}
