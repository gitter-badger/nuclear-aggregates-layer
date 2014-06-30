using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public interface IContextedMigration<in T> : IMigration where T : IMigrationContextBase
    {
        void Apply(T context);
        void Revert(T context);
    }
}
