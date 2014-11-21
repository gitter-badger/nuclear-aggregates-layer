using System;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    // todo {a.tukaev}: Я бы убрал такую деталь реализации, как sql таймаут внутрь PersistenceService
    public interface ICleanupPersistenceService : ISimplifiedPersistenceService
    {
        void CleanupErm(TimeSpan timeout, int logSizeInDays);
        void CleanupErmLogging(TimeSpan timeout, int logSizeInDays);
        void CleanupCrm(TimeSpan timeout, int logSizeInDays);
    }
}