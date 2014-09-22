using System;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    // todo: {a.tukaev}: Я бы убрал такую деталь реализации, как sql таймаут внутрь PersistenceService
    public interface IClientPersistenceService : IPersistenceService<Client>
    {
        EntityChangesContext CalculateClientPromising(long modifiedBy, TimeSpan timeout);
    }
}