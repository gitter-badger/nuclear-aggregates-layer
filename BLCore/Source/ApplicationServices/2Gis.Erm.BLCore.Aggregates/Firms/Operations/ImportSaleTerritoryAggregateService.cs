using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    // FIXME {all, 26.05.2014}: нарушение соглашений для AggregateService - сам себе читает данные, требуется рефакторинг + учесть проблемы с импортом удаленных территорий + batch режим работы
    public sealed class ImportSaleTerritoryAggregateService : IImportSaleTerritoryAggregateService
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
            var territory = _finder.Find(new FindSpecification<Territory>(t => t.Id == saleTerritoryDto.Code)).One() ??
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
        // COMMENT {all, 26.05.2014}: более правильным поведением было бы вызов из operationservice импорта территорий operationservice деактивации территорий (возможно, его batch вариант)
        //                          однако, для использования operationservice деактивации территорий нужно знать на какую территорию переносить все сущности связанные с декактивированной -
        //                          т.е. логичным выглядит изменить поток географий, также есть предложение убрать сервис деактивации терриорий из public API ERM - эта операция должна выполняться только в случае импорта 
        private void ProcessDeletedTerritory(SaleTerritoryServiceBusDto saleTerritoryDto)
        {
            var territory = _finder.Find(new FindSpecification<Territory>(t => t.Id == saleTerritoryDto.Code)).One();
            if (territory == null)
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, Territory>())
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
            return _finder.FindObsolete(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                          .Where(unit => unit.DgppId != null)
                          .Select(unit => new { unit.DgppId, unit.Id })
                          .ToDictionary(x => (int)x.DgppId, x => x.Id);
        }
    }
}