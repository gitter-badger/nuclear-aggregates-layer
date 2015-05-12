using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.MultiCulture.Firms.Operations
{
    public interface IImportCardAggregateService : IAggregatePartRepository<Firm>
    {
        EntityChangesContext ImportCards(IEnumerable<MultiCultureCardServiceBusDto> dtos,
                                      long userId,
                                      long reserveUserId,
                                      long[] pregeneratedIds,
                                      string regionalTerritoryLocaleSpecificWord,
                                      bool enableReplication);
    }
}