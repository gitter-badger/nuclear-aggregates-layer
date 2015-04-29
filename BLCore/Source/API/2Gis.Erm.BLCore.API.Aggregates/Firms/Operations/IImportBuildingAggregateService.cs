using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportBuildingAggregateService : IAggregatePartRepository<Firm>
    {
        void ImportBuildingFromServiceBus(IEnumerable<BuildingServiceBusDto> buildingDtos, string regionalTerritoryLocaleSpecificWord, bool enableReplication, bool useWarehouseIntegration);
    }
}