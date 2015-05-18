using DoubleGis.Erm.Platform.DAL;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IExportPersistenceService : ISimplifiedPersistenceService
    {
        bool TryCreateExportSession(IEntityType entityType, bool exportInvalidObjects, out long sessionId);
    }
}