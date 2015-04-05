using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface ICreateDeniedPositionAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        void Create(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition);
        void CreateSelfDeniedPosition(DeniedPosition selfDeniedPosition);

        /// <summary>
        /// Нужен для кейса редактирования правила запрещения со сменой направления c самозапрещения
        /// </summary>
        void Create(DeniedPosition deniedPosition);
    }
}