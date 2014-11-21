using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IExportPersistenceService : ISimplifiedPersistenceService
    {
        bool TryCreateExportSession(EntityName entityType, bool exportInvalidObjects, out long sessionId);
    }
}