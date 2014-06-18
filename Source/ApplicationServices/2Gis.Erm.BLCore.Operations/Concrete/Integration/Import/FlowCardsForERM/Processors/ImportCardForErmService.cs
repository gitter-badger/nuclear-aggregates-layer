using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCardsForERM.Processors
{
    // COMMENT {y.baranihin, 17.06.2014}: Юра, похоже у тебя настройки R# какие-то свои и они переопределяют командные
    public class ImportCardForErmService : IImportCardForErmService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IImportFirmAddressService _importFirmAddressService;
        private readonly IImportCategoryFirmAddressService _importCategoryFirmAddressService;
        private readonly IImportDepCardsService _importDepCardsService;
        private readonly IImportFirmContactsService _importFirmContactsService;

        public ImportCardForErmService(IOperationScopeFactory scopeFactory,
                                       IImportFirmAddressService importFirmAddressService,
                                       IImportCategoryFirmAddressService importCategoryFirmAddressService,
                                       IImportDepCardsService importDepCardsService,
                                       IImportFirmContactsService importFirmContactsService)
        {
            _scopeFactory = scopeFactory;
            _importFirmAddressService = importFirmAddressService;
            _importCategoryFirmAddressService = importCategoryFirmAddressService;
            _importDepCardsService = importDepCardsService;
            _importFirmContactsService = importFirmContactsService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardForErmServiceBusDtos = dtos.Cast<CardForErmServiceBusDto>().ToArray();

            var posCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Pos).ToArray();
            var depCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Dep).ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardForErmIdentity>())
            {
                _importDepCardsService.Import(depCards.Select(ToDepCard));

                _importFirmAddressService.Import(posCards.Select(ToImportFirmAddressDto));

                _importCategoryFirmAddressService.Import(posCards.SelectMany(x => x.Rubrics.Select(ToCategoryFirmAddress)),
                                                         posCards.Select(x => x.Code).ToArray());

                var depCardContactsToImport = depCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, null, x.Code)));

                var posCardContactsToImport = posCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, x.Code, null)));

                _importFirmContactsService.Import(depCardContactsToImport.Union(posCardContactsToImport),
                                                  posCards.Select(x => x.Code).Distinct(),
                                                  depCards.Select(x => x.Code).Distinct());

                scope.Updated<Firm>(cardForErmServiceBusDtos.Select(x => x.FirmCode).Distinct()).Complete();
            }
        }

        private static ImportFirmAddressDto ToImportFirmAddressDto(CardForErmServiceBusDto cardForErmServiceBusDto)
        {
            return new ImportFirmAddressDto
                {
                    FirmAddress = new FirmAddress
                        {
                            Id = cardForErmServiceBusDto.Code,
                            FirmId = cardForErmServiceBusDto.FirmCode,
                            Address = cardForErmServiceBusDto.Address.Text,
                            WorkingTime = cardForErmServiceBusDto.Schedule.Text,
                            PaymentMethods = cardForErmServiceBusDto.Payment.Text,
                            IsLocatedOnTheMap = cardForErmServiceBusDto.IsLinked,
                            TerritoryId = cardForErmServiceBusDto.Address.TerritoryCode,
                            ClosedForAscertainment = cardForErmServiceBusDto.ClosedForAscertainment,
                            IsActive = cardForErmServiceBusDto.IsActive,
                            IsDeleted = cardForErmServiceBusDto.IsDeleted,
                            ReferencePoint = null
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
    }
}