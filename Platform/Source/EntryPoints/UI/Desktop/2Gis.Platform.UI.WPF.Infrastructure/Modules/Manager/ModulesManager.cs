using System.Collections.Generic;
using System.Reflection;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Manager
{
    public sealed class ModulesManager : IModulesManager
    {
        private readonly IModulesFinder _modulesFinder;

        public ModulesManager(IModulesFinder modulesFinder)
        {
            _modulesFinder = modulesFinder;
        }

        public IEnumerable<ModulesDescriptor> FindModules()
        {
            return _modulesFinder.GetModulesDescriptors();
        }

        public void LoadModule(ModulesDescriptor descriptor)
        {
            // TODO {d.ivanov, 01.07.2013}: Верифицировать сборки с модулями на предмет загрузки в приложение
            var assemblyName = new AssemblyName(descriptor.ModulesContainerAssembly);
            Assembly.LoadFrom(_modulesFinder.GetAssemblyPath(assemblyName));
        }
    }
}