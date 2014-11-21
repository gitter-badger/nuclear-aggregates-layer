using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Manager
{
    public interface IModulesManager
    {
        IEnumerable<ModulesDescriptor> FindModules();
        void LoadModule(ModulesDescriptor descriptor);
    }
}
