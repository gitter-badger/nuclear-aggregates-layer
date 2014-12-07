using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards.Processors
{
    public class EmiratesImportCardService : IImportCardService, IEmiratesAdapted
    {
        private readonly ICreateBlankFirmsOperationService _createBlankFirmsOperationService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IImportCategoryFirmAddressService _importCategoryFirmAddressService;
        private readonly IImportDepCardsService _importDepCardsService;
        private readonly IImportFirmAddressService _importFirmAddressService;
        private readonly IImportFirmContactsService _importFirmContactsService;
        private readonly IPaymentMethodFormatter _paymentMethodFormatter;
        private readonly IPhoneNumbersFormatter _phoneNumbersFormatter;
        private readonly IOperationScopeFactory _scopeFactory;

        public EmiratesImportCardService(ICreateBlankFirmsOperationService createBlankFirmsOperationService,
                                         IFirmReadModel firmReadModel,
                                         IImportCategoryFirmAddressService importCategoryFirmAddressService,
                                         IImportDepCardsService importDepCardsService,
                                         IImportFirmAddressService importFirmAddressService,
                                         IImportFirmContactsService importFirmContactsService,
                                         IPaymentMethodFormatter paymentMethodFormatter,
                                         IPhoneNumbersFormatter phoneNumbersFormatter,
                                         IOperationScopeFactory scopeFactory)
        {
            _createBlankFirmsOperationService = createBlankFirmsOperationService;
            _firmReadModel = firmReadModel;
            _importCategoryFirmAddressService = importCategoryFirmAddressService;
            _importDepCardsService = importDepCardsService;
            _importFirmAddressService = importFirmAddressService;
            _importFirmContactsService = importFirmContactsService;
            _paymentMethodFormatter = paymentMethodFormatter;
            _phoneNumbersFormatter = phoneNumbersFormatter;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var posCards = dtos.OfType<EmiratesPosCardServiceBusDto>().ToArray();
            var depCards = dtos.OfType<EmiratesDepCardServiceBusDto>().ToArray();

            var unknownCardTypes = dtos.Except(posCards).Except(depCards).Select(x => x.GetType().Name).Distinct().ToArray();
            if (unknownCardTypes.Any())
            {
                throw new InvalidOperationException("Попытка импорта карточек неизвестного типа:" + string.Join(",", unknownCardTypes));
            }

            var phonesAndFaxes = depCards.SelectMany(x => x.Contacts)
                                         .Where(x => x.ContactType == ContactType.Phone || x.ContactType == ContactType.Fax)
                                         .Concat(posCards.SelectMany(x => x.Contacts)
                                                         .Where(x => x.ContactType == ContactType.Phone || x.ContactType == ContactType.Fax))
                                         .ToArray();

            _phoneNumbersFormatter.FormatPhoneNumbers(phonesAndFaxes);

            var formattedPaymentMethods = _paymentMethodFormatter.FormatPaymentMethods(posCards.ToDictionary(x => x.Code, x => x.PaymentMethodCodes));

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardIdentity>())
            {
                var branchCodesByFirmIds = posCards.GroupBy(x => x.FirmCode)
                                                   .ToDictionary(x => x.Key, x => x.Select(y => y.BranchCode).First());
                var existingFirms = _firmReadModel.GetFirms(branchCodesByFirmIds.Keys);
                var firmIdsToCreate = branchCodesByFirmIds.Keys.Except(existingFirms.Keys);

                _createBlankFirmsOperationService.CreateBlankFirms(firmIdsToCreate.Select(x => new BlankFirmDto
                                                                                                   {
                                                                                                       FirmId = x,
                                                                                                       BranchCode = branchCodesByFirmIds[x]
                                                                                                   }));
                _importDepCardsService.Import(depCards.Select(x => new DepCard
                                                                       {
                                                                           Id = x.Code,
                                                                           IsHiddenOrArchived = x.IsHiddenOrArchived
                                                                       }));

                _importFirmAddressService.Import(posCards.Select(x => ToImportFirmAddressDto(x, formattedPaymentMethods)));

                _importCategoryFirmAddressService.Import(posCards.SelectMany(x => x.Rubrics.Select(ToCategoryFirmAddress)),
                                                         posCards.Select(x => x.Code).ToArray());

                var depCardContactsToImport = depCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, null, x.Code)));

                var posCardContactsToImport = posCards.SelectMany(x => x.Contacts
                                                                        .Select(y => ToImportFirmContact(y, x.Code, null)));

                _importFirmContactsService.Import(depCardContactsToImport.Concat(posCardContactsToImport),
                                                  posCards.Select(x => x.Code).Distinct(),
                                                  depCards.Select(x => x.Code).Distinct());

                scope.Updated<Firm>(branchCodesByFirmIds.Keys)
                     .Complete();
            }
        }

        private static FirmContact ToImportFirmContact(ContactDto contactDto, long? firmAddressId, long? cardId)
        {
            return new FirmContact
                {
                    Contact = contactDto.Contact,
                    ContactType = (FirmAddressContactType)contactDto.ContactType,
                    SortingPosition = contactDto.SortingPosition,
                    FirmAddressId = firmAddressId,
                    CardId = cardId
                };
        }

        private static ImportFirmAddressDto ToImportFirmAddressDto(EmiratesPosCardServiceBusDto posCard,
                                                                   IReadOnlyDictionary<long, string> formattedPaymentMethods)
        {
            return new ImportFirmAddressDto
                {
                    FirmAddress = new FirmAddress
                        {
                            Id = posCard.Code,
                            FirmId = posCard.FirmCode,
                            ReferencePoint = posCard.ReferencePoint,
                            Address = posCard.Address,
                            AddressCode = posCard.AddressCode,
                            BuildingCode = posCard.BuildingCode,
                            ClosedForAscertainment = posCard.ClosedForAscertainment,
                            IsActive = posCard.IsActive,
                            IsDeleted = posCard.IsDeleted,
                            IsLocatedOnTheMap = posCard.IsLinkedToTheMap,
                            WorkingTime = posCard.Schedule,
                            PaymentMethods = formattedPaymentMethods[posCard.Code],
                            Parts = new[] { new EmiratesFirmAddressPart { PoBox = posCard.PoBox } }
                        },
                    BranchCode = posCard.BranchCode,
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
    }
}