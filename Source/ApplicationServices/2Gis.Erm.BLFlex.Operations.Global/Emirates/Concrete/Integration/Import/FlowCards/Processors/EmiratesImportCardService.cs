using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards.Processors
{
    public class EmiratesImportCardService : IImportCardService, IEmiratesAdapted
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPhoneNumbersFormatter _phoneNumbersFormatter;
        private readonly IPaymentMethodFormatter _paymentMethodFormatter;
        private readonly IImportFirmAddressService _importFirmAddressService;
        private readonly IImportCategoryFirmAddressService _importCategoryFirmAddressService;
        private readonly IImportDepCardsService _importDepCardsService;
        private readonly IImportFirmContactsService _importFirmContactsService;

        public EmiratesImportCardService(IOperationScopeFactory scopeFactory,
                                         IPhoneNumbersFormatter phoneNumbersFormatter,
                                         IPaymentMethodFormatter paymentMethodFormatter,
                                         IImportFirmAddressService importFirmAddressService,
                                         IImportCategoryFirmAddressService importCategoryFirmAddressService,
                                         IImportDepCardsService importDepCardsService,
                                         IImportFirmContactsService importFirmContactsService)
        {
            _scopeFactory = scopeFactory;
            _phoneNumbersFormatter = phoneNumbersFormatter;
            _paymentMethodFormatter = paymentMethodFormatter;
            _importFirmAddressService = importFirmAddressService;
            _importCategoryFirmAddressService = importCategoryFirmAddressService;
            _importDepCardsService = importDepCardsService;
            _importFirmContactsService = importFirmContactsService;
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
                var firmIds = posCards.Select(x => x.FirmCode).ToArray();

                _importDepCardsService.Import(depCards.Select(x =>
                                                              new DepCard
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

                scope.Updated<Firm>(firmIds)
                     .Complete();
            }
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