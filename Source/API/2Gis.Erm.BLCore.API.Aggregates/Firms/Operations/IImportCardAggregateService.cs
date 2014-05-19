using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportCardAggregateService : IAggregatePartRepository<Firm>
    {
        IEnumerable<long> ImportCards(IEnumerable<CardServiceBusDto> dtos,
                                              long userId,
                                              long reserveUserId,
                                              long[] pregeneratedIds,
                                              string regionalTerritoryLocaleSpecificWord,
                                              bool enableReplication);
    }
}