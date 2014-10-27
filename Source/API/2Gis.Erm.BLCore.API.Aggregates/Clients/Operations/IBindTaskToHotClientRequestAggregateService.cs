using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    // FIXME {s.pomadin, 27.10.2014}: Сервис операции должен реализовывать IOperation. Если же это aggregate-сервис, то он относится к write-модели и должен получать необходимые данные в параметрах
    public interface IBindTaskToHotClientRequestAggregateService : IAggregateSpecificOperation<Firm, BindTaskToHotClientRequestIdentity>
    {
        void BindTask(long requestId, Guid taskId);
    }
}