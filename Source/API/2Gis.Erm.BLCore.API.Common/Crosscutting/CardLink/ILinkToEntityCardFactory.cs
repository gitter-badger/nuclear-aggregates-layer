using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink
{
    public interface ILinkToEntityCardFactory : IInvariantSafeCrosscuttingService
    {
        Uri CreateLink<TEntity>(long entityId)
            where TEntity : class, IEntity;
    }
}
