using System;
using System.Collections.Generic;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCardsForERM.Deserializers
{
    public sealed class DeserializeCardForErmService : IDeserializeServiceBusObjectService<CardForErmServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var contacts = new List<ContactDto>();
            var rubrics = new List<ImportCategoryFirmAddressDto>();
            var schedule = new ScheduleDto();
            var payment = new PaymentDto();

            var cardCode = (long)xml.Attribute("Code");
            var cardType = (CardType)Enum.Parse(typeof(CardType), (string)xml.Attribute("Type"), true);

            var contactsElement = xml.Element("Contacts");
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

            var rubricsElement = xml.Element("Rubrics");
            if (rubricsElement != null && cardType == CardType.Pos)
            {
                foreach (var rubricElement in rubricsElement.Elements())
                {
                    rubrics.Add(new ImportCategoryFirmAddressDto
                        {
                            FirmAddressId = cardCode,
                            CategoryId = (int)rubricElement.Attribute("Code"),
                            IsPrimary = (bool?)rubricElement.Attribute("IsPrimary") ?? false,
                            SortingPosition = (int)rubricElement.Attribute("SortingPosition")
                        });
                }
            }

            var addressElement = xml.Element("Address");
            if (addressElement == null)
            {
                throw new NotificationException("Импорт карточек из потока CardForERM - отсутствует обязательный элемент Address");
            }

            var scheduleElement = xml.Element("Schedule");
            if (scheduleElement != null)
            {
                schedule = new ScheduleDto
                    {
                        Text = (string)scheduleElement.Attribute("Text")
                    };
            }

            var paymentElement = xml.Element("Payment");
            if (paymentElement != null)
            {
                payment = new PaymentDto
                    {
                        Text = (string)paymentElement.Attribute("Text")
                    };
            }

            var firmElement = xml.Element("Firm");
            if (firmElement == null)
            {
                throw new NotificationException("Импорт карточек из потока CardForERM - отсутствует обязательный элемент Firm");
            }

            return new CardForErmServiceBusDto
                {
                    Code = cardCode,
                    Type = cardType,
                    BranchCode = (int)xml.Attribute("BranchCode"),
                    IsActive = (bool)xml.Attribute("IsActive"),
                    ClosedForAscertainment = (bool)xml.Attribute("ClosedForAscertainment"),
                    IsLinked = (bool)xml.Attribute("IsLinked"),
                    IsDeleted = (bool)xml.Attribute("IsDeleted"),
                    Address = new CardAddressDto
                        {
                            TerritoryCode = (long?)addressElement.Attribute("TerritoryCode"),
                            Text = (string)addressElement.Attribute("Text")
                        },

                    SortingPosition = (int?)xml.Attribute("SortingPosition"),
                    Contacts = contacts,
                    Rubrics = rubrics,
                    Schedule = schedule,
                    Payment = payment,
                    Firm = new FirmForErmDto
                        {
                            Code = (long)firmElement.Attribute("Code"),
                            Name = (string)firmElement.Attribute("Name"),
                            BranchCode = (int)firmElement.Attribute("BranchCode"),
                            ClosedForAscertainment = (bool)firmElement.Attribute("ClosedForAscertainment"),
                            IsActive = (bool)firmElement.Attribute("IsActive")
                        }
                };
        }

        public bool Validate(XElement xml, out string error)
        {
            error = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}