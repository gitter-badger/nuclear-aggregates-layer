using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

using ImportFirmAddressDto = DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared.ImportFirmAddressDto;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCardsForERM.Processors
{
    public class ImportCardForErmService : IImportCardForErmService
    {
        private readonly IBulkChangeClientTerritoryAggregateService _bulkChangeClientTerritoryAggregateService;
        private readonly IBulkChangeFirmTerritoryAggregateService _bulkChangeFirmTerritoryAggregateService;
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IImportCategoryFirmAddressService _importCategoryFirmAddressService;
        private readonly IImportDepCardsService _importDepCardsService;
        private readonly IImportFirmAddressService _importFirmAddressService;
        private readonly IImportFirmContactsService _importFirmContactsService;
        private readonly IImportFirmsDuringImportCardsForErmOperationService _importFirmsDuringImportCardsForErmOperationService;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardForErmService(IBulkChangeClientTerritoryAggregateService bulkChangeClientTerritoryAggregateService,
                                       IBulkChangeFirmTerritoryAggregateService bulkChangeFirmTerritoryAggregateService,
                                       IClientReadModel clientReadModel,
                                       IFirmReadModel firmReadModel,
                                       IImportCategoryFirmAddressService importCategoryFirmAddressService,
                                       IImportDepCardsService importDepCardsService,
                                       IImportFirmAddressService importFirmAddressService,
                                       IImportFirmContactsService importFirmContactsService,
                                       IImportFirmsDuringImportCardsForErmOperationService importFirmsDuringImportCardsForErmOperationService,
                                       IIntegrationLocalizationSettings integrationLocalizationSettings,
                                       IOperationScopeFactory scopeFactory)
        {
            _bulkChangeClientTerritoryAggregateService = bulkChangeClientTerritoryAggregateService;
            _bulkChangeFirmTerritoryAggregateService = bulkChangeFirmTerritoryAggregateService;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _importCategoryFirmAddressService = importCategoryFirmAddressService;
            _importDepCardsService = importDepCardsService;
            _importFirmAddressService = importFirmAddressService;
            _importFirmContactsService = importFirmContactsService;
            _importFirmsDuringImportCardsForErmOperationService = importFirmsDuringImportCardsForErmOperationService;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardForErmServiceBusDtos = dtos.Cast<CardForErmServiceBusDto>().ToArray();

            var posCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Pos).ToArray();
            var depCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Dep).ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardForErmIdentity>())
            {
                var firmsToImport = posCards.Select(x => x.Firm).DistinctBy(x => x.Code).ToArray();
                _importFirmsDuringImportCardsForErmOperationService.Import(firmsToImport);

                _importDepCardsService.Import(depCards.Select(ToDepCard));

                _importFirmAddressService.Import(posCards.Select(ToImportFirmAddressDto));

                UpdateFirmAndClientTerritory(firmsToImport.Select(x => x.Code));

                _importCategoryFirmAddressService.Import(posCards.SelectMany(x => x.Rubrics.Select(ToCategoryFirmAddress)),
                                                         posCards.Select(x => x.Code).ToArray());

                var depCardContactsToImport = depCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, null, x.Code)));

                var posCardContactsToImport = posCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, x.Code, null)));

                _importFirmContactsService.Import(depCardContactsToImport.Concat(posCardContactsToImport),
                                                  posCards.Select(x => x.Code).Distinct(),
                                                  depCards.Select(x => x.Code).Distinct());

                scope.Updated<Firm>(cardForErmServiceBusDtos.Select(x => x.Firm.Code).Distinct()).Complete();
            }
        }

        private static ImportFirmAddressDto ToImportFirmAddressDto(CardForErmServiceBusDto cardForErmServiceBusDto)
        {
            return new ImportFirmAddressDto
                {
                    FirmAddress = new FirmAddress
                        {
                            Id = cardForErmServiceBusDto.Code,
                            FirmId = cardForErmServiceBusDto.Firm.Code,
                            Address = cardForErmServiceBusDto.Address.Text,
                            WorkingTime = cardForErmServiceBusDto.Schedule.Text,
                            PaymentMethods = cardForErmServiceBusDto.Payment.Text,
                            IsLocatedOnTheMap = cardForErmServiceBusDto.IsLinked,
                            TerritoryId = cardForErmServiceBusDto.Address.TerritoryCode,
                            ClosedForAscertainment = cardForErmServiceBusDto.ClosedForAscertainment,
                            IsActive = cardForErmServiceBusDto.IsActive,
                            IsDeleted = cardForErmServiceBusDto.IsDeleted,
                            // ReSharper disable once PossibleInvalidOperationException 
                            SortingPosition = cardForErmServiceBusDto.SortingPosition.Value,
                            ReferencePoint = null,
                            AddressCode = null,
                            BuildingCode = null
                        },
                    BranchCode = cardForErmServiceBusDto.BranchCode
                };
        }

        private static DepCard ToDepCard(CardForErmServiceBusDto cardForErmServiceBusDto)
        {
            return new DepCard
                {
                    Id = cardForErmServiceBusDto.Code,
                    IsHiddenOrArchived = !cardForErmServiceBusDto.IsActive ||
                                         cardForErmServiceBusDto.IsDeleted ||
                                         cardForErmServiceBusDto.ClosedForAscertainment
                };
        }

        private static CategoryFirmAddress ToCategoryFirmAddress(ImportCategoryFirmAddressDto cardRubricDto)
        {
            return new CategoryFirmAddress
                {
                    FirmAddressId = cardRubricDto.FirmAddressId,
                    CategoryId = cardRubricDto.CategoryId,
                    IsPrimary = cardRubricDto.IsPrimary,
                    SortingPosition = cardRubricDto.SortingPosition
                };
        }

        private static FirmContact ToImportFirmContact(ContactDto contactDto, long? firmAddressId, long? cardId)
        {
            return new FirmContact
                {
                    Contact = contactDto.Contact,
                    ContactType = (int)contactDto.ContactType,
                    SortingPosition = contactDto.SortingPosition,
                    FirmAddressId = firmAddressId,
                    CardId = cardId
                };
        }

        private void UpdateFirmAndClientTerritory(IEnumerable<long> firmIds)
        {
            var firms = _firmReadModel.GetFirms(firmIds);
            var firmTerritories = _firmReadModel.GetFirmTerritories(firmIds, _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord);

            var firmsToUpdate = firms.Values.Select(firm => new ChangeFirmTerritoryDto { Firm = firm, TerritoryId = firmTerritories[firm.Id] }).ToArray();

            _bulkChangeFirmTerritoryAggregateService.ChangeTerritory(firmsToUpdate);

            var clientsByFirmIds = _clientReadModel.GetClientsToUpdateTerritoryByFirms(firmsToUpdate.Select(x => x.Firm.Id));


            var clientsToUpdate = clientsByFirmIds.SelectMany(clientsByFirm => clientsByFirm.Value
                                                                                            .Select(client => new ChangeClientTerritoryDto
                                                                                                {
                                                                                                    Client = client,
                                                                                                    TerritoryId = firmTerritories[clientsByFirm.Key]
                                                                                                }))
                                                  .ToArray();

            _bulkChangeClientTerritoryAggregateService.ChangeTerritory(clientsToUpdate);
        }
    }
}