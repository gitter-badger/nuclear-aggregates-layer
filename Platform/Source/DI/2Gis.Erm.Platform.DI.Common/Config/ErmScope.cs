using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public sealed class ErmScope : DependencyScope
    {
        public ErmScope() : base(Mapping.Erm)
        {
        }
    }
}