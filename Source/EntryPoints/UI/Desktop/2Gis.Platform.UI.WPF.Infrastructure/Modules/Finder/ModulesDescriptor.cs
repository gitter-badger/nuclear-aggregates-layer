using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder
{
    [Serializable]
    public sealed class ModulesDescriptor
    {
        public string ModulesContainerFullPath { get; set; }
        public string ModulesContainerConfigFullPath { get; set; }
        public string ModulesContainerAssembly { get; set; }
        public string ContainerType { get; set; }
        public string[] ModuleTypes { get; set; }
        public SettingsDescriptor[] Settings { get; set; }
    }
}
