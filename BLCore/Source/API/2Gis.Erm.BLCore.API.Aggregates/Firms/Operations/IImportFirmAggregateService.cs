using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportFirmAggregateService : IAggregatePartService<Firm>
    {
        EntityChangesContext ImportFirms(IEnumerable<FirmServiceBusDto> dtos,
                                      long userId,
                                      long reserveUserId,
                                      string regionalTerritoryLocaleSpecificWord,
                                      bool enableReplication);
    }
}