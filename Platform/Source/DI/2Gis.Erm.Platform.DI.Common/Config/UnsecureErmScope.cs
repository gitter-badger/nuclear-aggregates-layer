using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public sealed class UnsecureErmScope : DependencyScope
    {
        public UnsecureErmScope() : base(Mapping.UnsecureErm)
        {
        }
    }
}