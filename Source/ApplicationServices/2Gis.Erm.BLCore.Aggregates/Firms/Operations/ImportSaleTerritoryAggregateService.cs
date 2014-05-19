﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportSaleTerritoryAggregateService : IImportSaleTerritoryAggregateService
    {
        private readonly IFinder _finder;
        private readonly IRepository<Territory> _territoryRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportSaleTerritoryAggregateService(IFinder finder, IRepository<Territory> territoryRepository, IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _territoryRepository = territoryRepository;
            _scopeFactory = scopeFactory;
        }

        public void ImportTerritoryFromServiceBus(IEnumerable<SaleTerritoryServiceBusDto> territoryDtos)
        {
            var territoryServiceBusDtos = territoryDtos as SaleTerritoryServiceBusDto[] ?? territoryDtos.ToArray();
            if (!territoryServiceBusDtos.Any())
            {
                return;
            }

            var context = GetOrganizationUnits();
            foreach (var territoryDto in territoryServiceBusDtos.Where(dto => !dto.IsDeleted))
            {
                ProcessActiveTerritory(territoryDto, context);
            }

            foreach (var territoryDto in territoryServiceBusDtos.Where(dto => dto.IsDeleted))
            {
                ProcessDeletedTerritory(territoryDto);
            }

            _territoryRepository.Save();
        }

        private void ProcessActiveTerritory(SaleTerritoryServiceBusDto saleTerritoryDto, IDictionary<int, long> dgppToErmIds)
        {
            var territory = _finder.Find<Territory>(t => t.Id == saleTerritoryDto.Code).SingleOrDefault() ??
                            new Territory { Id = saleTerritoryDto.Code };

            long organizationUnitId;
            if (!dgppToErmIds.TryGetValue(saleTerritoryDto.OrganizationUnitDgppId, out organizationUnitId))
            {
                throw new ArgumentException(string.Format("Импорт. Не найдено отделние организации с кодом '{0}'", saleTerritoryDto.OrganizationUnitDgppId));
            }

            territory.Name = saleTerritoryDto.Name;
            territory.IsActive = !saleTerritoryDto.IsDeleted;
            territory.OrganizationUnitId = organizationUnitId;

            if (territory.IsNew())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Territory>())
                {
                    _territoryRepository.Add(territory);
                    scope.Added<Territory>(territory.Id)
                         .Complete();
                }
            }
            else
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Territory>())
                {
                    _territoryRepository.Update(territory);
                    scope.Updated<Territory>(territory.Id)
                         .Complete();
                }
            }
        }

        // FIXME {all, 16.12.2013}: А как-же пользователи, фирмы и прочее, привязанное к этой территории? Нельзя просто так взять и деактивировать территорию. 
        //                          Мы так поступаем уже давно и решили не менять логику в рамках задачи по изменению схемы. Тем не менее когда-то это нужно сделать.
        private void ProcessDeletedTerritory(SaleTerritoryServiceBusDto saleTerritoryDto)
        {
            var territory = _finder.Find<Territory>(t => t.Id == saleTerritoryDto.Code).SingleOrDefault();
            if (territory == null)
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Territory>())
            {
                territory.IsActive = false;
                _territoryRepository.Update(territory);
                scope.Updated<Territory>(territory.Id)
                     .Complete();
            }
        }

        // ключ - Дгпп, значение - Ерм
        private IDictionary<int, long> GetOrganizationUnits()
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                          .Where(unit => unit.DgppId != null)
                          .Select(unit => new { unit.DgppId, unit.Id })
                          .ToDictionary(x => (int)x.DgppId, x => x.Id);
        }
    }
}