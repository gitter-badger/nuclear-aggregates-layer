﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportSaleTerritoryAggregateService : IAggregatePartRepository<Firm>
    {
        void ImportTerritoryFromServiceBus(IEnumerable<SaleTerritoryServiceBusDto> territoryDtos);
    }
}