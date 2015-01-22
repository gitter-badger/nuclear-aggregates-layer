using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink
{
    public interface ILinkToEntityCardFactory : IInvariantSafeCrosscuttingService
    {
        Uri CreateLink<TEntity>(long entityId)
            where TEntity : class, IEntity;
    }
}
