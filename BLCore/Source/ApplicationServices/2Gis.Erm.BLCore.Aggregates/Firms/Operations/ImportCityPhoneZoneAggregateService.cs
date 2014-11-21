﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportCityPhoneZoneAggregateService : IImportCityPhoneZoneAggregateService
    {
        private readonly IRepository<CityPhoneZone> _cityPhoneZoneGenericRepository;
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCityPhoneZoneAggregateService(IOperationScopeFactory scopeFactory,
                                                   IRepository<CityPhoneZone> cityPhoneZoneGenericRepository,
                                                   IFinder finder)
        {
            _scopeFactory = scopeFactory;
            _cityPhoneZoneGenericRepository = cityPhoneZoneGenericRepository;
            _finder = finder;
        }

        public void ImportCityPhoneZonesFromServiceBus(IEnumerable<CityPhoneZone> cityPhoneZones)
        {
            foreach (var cityPhoneZone in cityPhoneZones)
            {
                var cityPhoneZoneCode = cityPhoneZone.Id;

                var cityPhoneZoneExists = _finder.Find<CityPhoneZone>(x => x.Id == cityPhoneZoneCode).Any();
                if (cityPhoneZoneExists)
                {
                    return;
                }

                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, CityPhoneZone>())
                {
                    _cityPhoneZoneGenericRepository.Add(cityPhoneZone);
                    scope.Added<CityPhoneZone>(cityPhoneZone.Id)
                         .Complete();
                }
            }

            _cityPhoneZoneGenericRepository.Save();
        }
    }
}