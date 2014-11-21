using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.PhoneZones;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowPhoneZones.Processors
{
    public class ImportCityPhoneZoneService : IImportCityPhoneZoneService
    {
        private readonly IImportCityPhoneZoneAggregateService _importCityPhoneZoneAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCityPhoneZoneService(IImportCityPhoneZoneAggregateService importCityPhoneZoneAggregateService,
                                          IOperationScopeFactory scopeFactory)
        {
            _importCityPhoneZoneAggregateService = importCityPhoneZoneAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cityPhoneZoneServiceBusDtos = dtos.Cast<CityPhoneZoneServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCityPhoneZoneIdentity>())
            {
                _importCityPhoneZoneAggregateService.ImportCityPhoneZonesFromServiceBus(cityPhoneZoneServiceBusDtos.Select(CreateCityPhoneZone));
                scope.Complete();
            }
        }

        // TODO {all, 10.04.2014}: Use ValueInjector?
        private static CityPhoneZone CreateCityPhoneZone(CityPhoneZoneServiceBusDto dto)
        {
            return new CityPhoneZone
                {
                    Id = dto.Id,
                    CityCode = dto.CityCode,
                    IsDefault = dto.IsDefault,
                    IsDeleted = dto.IsDeleted,
                    Name = dto.Name
                };
        }
    }
}