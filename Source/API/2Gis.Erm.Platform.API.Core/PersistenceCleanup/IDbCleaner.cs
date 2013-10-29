using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.PersistenceCleanup
{
    // FIXME {v.sinitsyn, 19.03.2013}: DbCleaner не имеет отношения к упрощенной модели приложения. Имеет смысл использовать ICleanupPersistenceService непосредственно в точке запуска use case-а
    public interface IDbCleaner : ISimplifiedModelConsumer
    {
        void CleanupErm();
        void CleanupCrm();
    }
}
