using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.CardForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public sealed class ImportFlowCardsForErmOperationService : IImportFlowCardsForErmOperationService
    {
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ImportFlowCardsForErmOperationService(IClientProxyFactory clientProxyFactory,
                                                     IIntegrationSettings integrationSettings,
                                                     IFirmRepository firmRepository,
                                                     IOperationScopeFactory scopeFactory,
                                                     IIntegrationLocalizationSettings integrationLocalizationSettings,
                                                     ICommonLog logger)
        {
            _clientProxyFactory = clientProxyFactory;
            _integrationSettings = integrationSettings;
            _firmRepository = firmRepository;
            _scopeFactory = scopeFactory;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _logger = logger;
        }

        #region enums

        private enum CardType
        {
            Pos,
            Dep
        }

        // Should be in sync with FirmAddressContactType
        private enum ContactType
        {
            // ReSharper disable UnusedMember.Local (used implicitly)
            None = 0,
            Phone = 1,
            Fax = 2,
            Email = 3,
            Web = 4,
            Icq = 5,
            Skype = 6,
            Jabber = 7 // == Other
            // ReSharper restore UnusedMember.Local
        }

        #endregion

        public void Import()
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            // TODO {d.ivanov;a.tukaev, 26.11.2013}: Скорее всего стоит разделять получение данных из шины и выполнение каких-то действий с данными. Может быть, это реализовать в виде двух разных операций?
            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowCardsForERM");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт карточек из потока CardsForERM - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт карточек из потока CardsForERM - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            ProcessPackage(package);
                        }

                        brokerApiReceiver.Acknowledge();
                    }
                }
                finally
                {
                    brokerApiReceiver.EndReceiving();
                }
            });
        }

        private void ProcessPackage(IEnumerable<string> package)
        {
            var cardDtos = package.Select(obj => XDocument.Parse(obj).Root)
                                  .Where(root => root != null && root.Name.LocalName == "CardForERM")
                                  .Select(CardForErmDto.Parse)
                                  .ToArray();

            var posCards = cardDtos.Where(x => x.Type == CardType.Pos).ToArray();
            var depCards = cardDtos.Where(x => x.Type == CardType.Dep).ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportFlowCardsForErmIdentity>())
            {
                _firmRepository.ImportDepCards(depCards.Select(x => x.ToImportDepCardDto()));

                foreach (var depCard in depCards)
                {
                    _firmRepository.ImportFirmContacts(depCard.Code, depCard.Contacts.Select(x => x.ToImportFirmContactDto()), true);
                }

                _firmRepository.ImportFirmAddresses(posCards.Select(x => x.ToImportFirmAddressDto()), _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord);

                foreach (var posCard in posCards)
                {
                    _firmRepository.ImportFirmContacts(posCard.Code, posCard.Contacts.Select(x => x.ToImportFirmContactDto()), false);
                    _firmRepository.ImportCategoryFirmAddresses(posCard.Code, posCard.Rubrics.Select(x => x.ToCategoryFirmAddressDto()));
                }

                scope.Updated<Firm>(cardDtos.Select(x => x.FirmCode).Distinct()).Complete();
            }
        }

        // TODO {a.tukaev, 26.11.2013}: Не забывай про StyleCop - он тут много чего подчеркивает
        // DONE {d.ivanov, 04.12.2013}: Ок
        #region dtos

        private class CardForErmDto
        {
            public long Code { get; private set; }
            public CardType Type { get; private set; }
            public IEnumerable<ContactDto> Contacts { get; private set; }
            public IEnumerable<CardRubricDto> Rubrics { get; private set; }

            private CardAddressDto Address { get; set; }
            private ScheduleDto Schedule { get; set; }
            private PaymentDto Payment { get; set; }
            public long FirmCode { get; set; }
            private int BranchCode { get; set; }
            private bool IsActive { get; set; }
            private bool ClosedForAscertainment { get; set; }
            private bool IsLinked { get; set; }
            private bool IsDeleted { get; set; }

            public static CardForErmDto Parse(XElement element)
            {
                var contacts = new List<ContactDto>();
                var rubrics = new List<CardRubricDto>();
                var schedule = new ScheduleDto();
                var payment = new PaymentDto();

                var contactsElement = element.Element("Contacts");
                if (contactsElement != null)
                {
                    foreach (var contactElement in contactsElement.Elements())
                    {
                        ContactType contactType;
                        if (!Enum.TryParse(contactElement.Name.LocalName, true, out contactType))
                        {
                            throw new NotificationException(string.Format("Импорт карточек из потока CardForERM - неизвестный тип контакта {0}",
                                                                          contactElement.Name.LocalName));
                        }

                        contacts.Add(new ContactDto
                        {
                            SortingPosition = (int)contactElement.Attribute("SortingPosition"),
                            Contact = (string)contactElement.Attribute("Value"),
                            ContactType = contactType
                        });
                    }
                }

                var rubricsElement = element.Element("Rubrics");
                if (rubricsElement != null)
                {
                    foreach (var rubricElement in rubricsElement.Elements())
                    {
                        rubrics.Add(new CardRubricDto
                        {
                            Code = (int)rubricElement.Attribute("Code"),
                            IsPrimary = (bool?)rubricElement.Attribute("IsPrimary") ?? false,
                            SortingPosition = (int)rubricElement.Attribute("SortingPosition")
                        });
                    }
                }

                var addressElement = element.Element("Address");
                if (addressElement == null)
                {
                    throw new NotificationException("Импорт карточек из потока CardForERM - отсутствует обязательный элемент Address");
                }

                var scheduleElement = element.Element("Schedule");
                if (scheduleElement != null)
                {
                    schedule = new ScheduleDto
                    {
                        Text = (string)scheduleElement.Attribute("Text")
                    };
                }

                var paymentElement = element.Element("Payment");
                if (paymentElement != null)
                {
                    payment = new PaymentDto
                    {
                        Text = (string)paymentElement.Attribute("Text")
                    };
                }

                return new CardForErmDto
                {
                    Code = (long)element.Attribute("Code"),
                    FirmCode = (long)element.Attribute("FirmCode"),
                    Type = (CardType)Enum.Parse(typeof(CardType), (string)element.Attribute("Type"), true),
                    BranchCode = (int)element.Attribute("BranchCode"),
                    IsActive = (bool)element.Attribute("IsActive"),
                    ClosedForAscertainment = (bool)element.Attribute("ClosedForAscertainment"),
                    IsLinked = (bool)element.Attribute("IsLinked"),
                    IsDeleted = (bool)element.Attribute("IsDeleted"),
                    Address = new CardAddressDto
                    {
                        TerritoryCode = (long?)addressElement.Attribute("TerritoryCode"),
                        Text = (string)addressElement.Attribute("Text")
                    },

                    Contacts = contacts,
                    Rubrics = rubrics,
                    Schedule = schedule,
                    Payment = payment
                };
            }

            public ImportFirmAddressDto ToImportFirmAddressDto()
            {
                return new ImportFirmAddressDto
                {
                    Code = Code,
                    BranchCode = BranchCode,
                    Address = Address.Text,
                    TerritoryCode = Address.TerritoryCode,
                    FirmCode = FirmCode,
                    IsActive = IsActive,
                    ClosedForAscertainment = ClosedForAscertainment,
                    IsDeleted = IsDeleted,
                    IsLinked = IsLinked,
                    Payment = Payment.Text,
                    Schedule = Schedule.Text
                };
            }

            public ImportDepCardDto ToImportDepCardDto()
            {
                return new ImportDepCardDto
                {
                    Code = Code,
                    IsHiddenOrArchived = !IsActive || IsDeleted || ClosedForAscertainment
                };
            }
        }

        private class ContactDto
        {
            public ContactType ContactType { private get; set; }
            public string Contact { private get; set; }
            public int SortingPosition { private get; set; }

            public ImportFirmContactDto ToImportFirmContactDto()
            {
                return new ImportFirmContactDto
                {
                    Contact = Contact,
                    ContactType = (int)ContactType,
                    SortingPosition = SortingPosition
                };
            }
        }

        private class CardAddressDto
        {
            public long? TerritoryCode { get; set; }
            public string Text { get; set; }
        }

        private class CardRubricDto
        {
            public int Code { private get; set; }
            public bool IsPrimary { private get; set; }
            public int SortingPosition { private get; set; }

            public ImportCategoryFirmAddressDto ToCategoryFirmAddressDto()
            {
                return new ImportCategoryFirmAddressDto
                {
                    Code = Code,
                    IsPrimary = IsPrimary,
                    SortingPosition = SortingPosition
                };
            }
        }

        private class ScheduleDto
        {
            public string Text { get; set; }
        }

        private class PaymentDto
        {
            public string Text { get; set; }
        }

        #endregion
    }
}