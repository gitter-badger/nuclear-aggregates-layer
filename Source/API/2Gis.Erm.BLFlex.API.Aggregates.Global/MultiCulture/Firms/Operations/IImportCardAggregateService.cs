using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.MultiCulture.Firms.Operations
{
    public interface IImportCardAggregateService : IAggregatePartRepository<Firm>
    {
        IEnumerable<long> ImportCards(IEnumerable<MultiCultureCardServiceBusDto> dtos,
                                      long userId,
                                      long reserveUserId,
                                      long[] pregeneratedIds,
                                      string regionalTerritoryLocaleSpecificWord,
                                      bool enableReplication);
    }
}