using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportCityPhoneZoneAggregateService : IAggregatePartRepository<Firm>
    {
        void ImportCityPhoneZonesFromServiceBus(IEnumerable<CityPhoneZone> cityPhoneZones);
    }
}