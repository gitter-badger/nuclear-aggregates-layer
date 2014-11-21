using System.Collections.Generic;
using System.Reflection;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder
{
    public interface IModulesFinder
    {
        IEnumerable<ModulesDescriptor> GetModulesDescriptors();
        string GetAssemblyPath(AssemblyName assemblyName);
    }
}