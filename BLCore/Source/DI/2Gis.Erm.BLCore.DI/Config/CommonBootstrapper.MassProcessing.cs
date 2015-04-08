using System.Reflection;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static bool IsErmAssembly(AssemblyName checkingAssemblyName)
        {
            const string ErmAssemblyNameTemplate = "2Gis.";

            return checkingAssemblyName.Name.StartsWith(ErmAssemblyNameTemplate);
        }

        public static bool IsErmAssembly(Assembly checkingAssembly)
        {
            return IsErmAssembly(checkingAssembly.GetName());
        }
    }
}