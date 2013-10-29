namespace DoubleGis.Erm.Platform.Migration.Runner
{
    internal static class EnvironmentUtil
    {
        public static string GetMsCrmDatabaseName(string environmentNumberString)
        {
            return string.Format("DoubleGis{0}_MSCRM", environmentNumberString);
        }
    }
}
