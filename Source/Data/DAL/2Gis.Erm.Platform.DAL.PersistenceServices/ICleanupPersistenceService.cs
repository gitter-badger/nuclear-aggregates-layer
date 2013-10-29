namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    // todo {a.tukaev}: Я бы убрал такую деталь реализации, как sql таймаут внутрь PersistenceService
    public interface ICleanupPersistenceService : ISimplifiedPersistenceService
    {
        void CleanupErm(int timeout, int logSizeInDays);
        void CleanupErmLogging(int timeout, int logSizeInDays);
        void CleanupCrm(int timeout, int logSizeInDays);
    }
}