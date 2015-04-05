using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    /// <summary>
    /// Нужен для кейса редактирования правила запрещения со сменой направления на самозапрещение
    /// </summary>
    public interface IDeleteDeniedPositionAggregateService : IAggregateSpecificOperation<Price, DeleteIdentity>
    {
        void Delete(DeniedPosition deniedPosition);
    }
}