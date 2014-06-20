using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class EmiratesDeserializeCardService : IDeserializeServiceBusObjectService<CardServiceBusDto>, IEmiratesAdapted
    {
        #region Константы

        private const string CodeAttributeName = "Code";
        private const string FirmCodeAttributeName = "FirmCode";
        private const string BranchCodeAttributeName = "BranchCode";
        private const string TypeAttributeName = "Type";
        private const string IsHiddenAttributeName = "IsHidden";
        private const string IsArchivedAttributeName = "IsArchived";
        private const string IsDeletedAttributeName = "IsDeleted";
        private const string CanReceiveFaxAttributeName = "CanReceiveFax";
        private const string ZoneCodeAttributeName = "ZoneCode";
        private const string IsLocalAttributeName = "IsLocal";
        private const string FormatCodeAttributeName = "FormatCode";
        private const string ContactValueAttributeName = "Value";
        private const string ContactIsHiddenAttributeName = "IsHidden";
        private const string ContactIsArchivedAttributeName = "IsArchived";
        private const string ContactNotPublishAttributeName = "NotPublish";
        private const string AddressCodeAttributeName = "AddressCode";
        private const string AddressTextAttributeName = "Text";
        private const string AddressReferencePointAttributeName = "ReferencePoint";
        private const string AddressPromotionalReferencePointAttributeName = "PromotionalReferencePoint";
        private const string BuildingCodeAttributeName = "BuildingCode";
        private const string RubricCodedAttributeName = "Code";
        private const string RubricIsPrimaryAttributeName = "IsPrimary";
        private const string DayLabelAttributeName = "Label";
        private const string WorkDayBeginTimeAttributeName = "From";
        private const string WorkDayEndTimeAttributeName = "To";
        private const string BreakBeginTimeAttributeName = "From";
        private const string BreakEndTimeAttributeName = "To";
        private const string ValueAttributeName = "Value";

        private const string ContactsElementName = "Contacts";
        private const string RubricsElementName = "Rubrics";
        private const string AddressElementName = "Address";
        private const string ItemsElementName = "Items";
        private const string ItemElementName = "Item";
        private const string FieldsElementName = "Fields";
        private const string TextFieldElementName = "TextField";
        private const string ReferenceListFieldElementName = "ReferenceListField";
        private const string SchedulesElementName = "Schedules";
        private const string ScheduleElementName = "Schedule";
        private const string DayElementName = "Day";
        private const string BreakElementName = "Break";

        private const string PoBoxCode = "PO Box";
        private const string PaymentMethodCode = "PaymentMethod";

        #endregion

        public IServiceBusDto Deserialize(XElement xml)
        {
            var typeAttr = xml.Attribute(TypeAttributeName);
            var type = (CardType)Enum.Parse(typeof(CardType), typeAttr.Value, true);

            switch (type)
            {
                case CardType.Pos:
                    return ParsePosCard(xml);
                case CardType.Dep:
                    return ParseDepCard(xml);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Validate(XElement xml, out string errorsMessage)
        {
            const string RequiredAttributeNotFound = "Отсутсвует обязательный атрибут '{0}'";
            var errorsBuilder = new StringBuilder();

            var codeAttr = xml.Attribute(CodeAttributeName);
            if (codeAttr == null)
            {
                errorsBuilder.AppendLine(string.Format(RequiredAttributeNotFound, CodeAttributeName));
            }

            var typeAttr = xml.Attribute(TypeAttributeName);
            if (typeAttr == null)
            {
                errorsBuilder.AppendLine(string.Format(RequiredAttributeNotFound, TypeAttributeName));
                errorsMessage = errorsBuilder.ToString();
                return false;
            }

            CardType type;
            if (!Enum.TryParse(typeAttr.Value, true, out type))
            {
                errorsBuilder.AppendLine(string.Format("Неизвестное значение атрибута '{0}'", TypeAttributeName));
            }
            else if (type == CardType.Pos)
            {
                var firmCodeAttr = xml.Attribute(FirmCodeAttributeName);
                if (firmCodeAttr == null)
                {
                    errorsBuilder.AppendLine(string.Format(RequiredAttributeNotFound, FirmCodeAttributeName));
                }

                var branchCodeAttr = xml.Attribute(BranchCodeAttributeName);
                if (branchCodeAttr == null)
                {
                    errorsBuilder.AppendLine(string.Format(RequiredAttributeNotFound, BranchCodeAttributeName));
                }
            }

            errorsMessage = errorsBuilder.ToString();

            return string.IsNullOrEmpty(errorsMessage);
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }

        private static EmiratesDepCardServiceBusDto ParseDepCard(XElement xml)
        {
            var result = new EmiratesDepCardServiceBusDto
                {
                    Code = (long)xml.Attribute(CodeAttributeName),
                    Contacts = ParseContacts(xml.Element(ContactsElementName)),
                    IsHiddenOrArchived = ((bool?)xml.Attribute(IsHiddenAttributeName) ?? false) ||
                                         ((bool?)xml.Attribute(IsArchivedAttributeName) ?? false) ||
                                         ((bool?)xml.Attribute(IsDeletedAttributeName) ?? false)
                };

            return result;
        }

        private static EmiratesPosCardServiceBusDto ParsePosCard(XElement xml)
        {
            var addressElement = xml.Element(AddressElementName);
            var schedulesElement = xml.Element(SchedulesElementName);
            var cardCode = (long)xml.Attribute(CodeAttributeName);

            var result = new EmiratesPosCardServiceBusDto
                {
                    Code = cardCode,
                    FirmCode = (long)xml.Attribute(FirmCodeAttributeName),
                    BranchCode = (int)xml.Attribute(BranchCodeAttributeName),
                    Contacts = ParseContacts(xml.Element(ContactsElementName)),
                    Rubrics = ParseRubrics(xml.Element(RubricsElementName), cardCode),
                    IsActive = !((bool?)xml.Attribute(IsArchivedAttributeName) ?? false),
                    IsDeleted = (bool?)xml.Attribute(IsDeletedAttributeName) ?? false,
                    ClosedForAscertainment = (bool?)xml.Attribute(IsHiddenAttributeName) ?? false,
                    AddressCode = (long?)addressElement.Attribute(AddressCodeAttributeName),
                    BuildingCode = (long?)addressElement.Attribute(BuildingCodeAttributeName),
                    Address = (string)addressElement.Attribute(AddressTextAttributeName),
                    ReferencePoint = (string)addressElement.Attribute(AddressPromotionalReferencePointAttributeName) ??
                                     (string)addressElement.Attribute(AddressReferencePointAttributeName),
                    PaymentMethodCodes = ParsePaymentMethodCodes(xml.Element(FieldsElementName)),
                    Schedule = schedulesElement != null ? ParseSchedule(schedulesElement.Element(ScheduleElementName)) : null,
                    PoBox = ParsePoBox(xml.Element(FieldsElementName)),
                    IsLinkedToTheMap = addressElement.Attribute(BuildingCodeAttributeName) != null &&
                                       (xml.Attribute(IsLocalAttributeName) == null || (bool)xml.Attribute(IsLocalAttributeName))
                };

            return result;
        }

        private static IEnumerable<ContactDto> ParseContacts(XContainer contactsElement)
        {
            if (contactsElement == null)
            {
                return Enumerable.Empty<ContactDto>();
            }

            var contacts = new List<ContactDto>();
            var sortingPositionCounter = 0;

            foreach (var contactElement in contactsElement.Elements())
            {
                var isContactHidden = ((bool?)contactElement.Attribute(ContactIsHiddenAttributeName) ?? false) ||
                                      ((bool?)contactElement.Attribute(ContactIsArchivedAttributeName) ?? false) ||
                                      ((bool?)contactElement.Attribute(ContactNotPublishAttributeName) ?? false);

                if (isContactHidden)
                {
                    continue;
                }

                sortingPositionCounter++;
                ContactType contactType;
                if (!Enum.TryParse(contactElement.Name.LocalName, true, out contactType))
                {
                    // COMMENT {all, 23.05.2014}: повторяю текущую логику - если пришел неизвестный тип контакта - это Jabber
                    // вероятно, это рано или поздно поменяется
                    contactType = ContactType.Jabber;
                }

                // телефон с функцией факса - это факс в понимании ERM
                if (contactType == ContactType.Phone && (bool?)contactElement.Attribute(CanReceiveFaxAttributeName) == true)
                {
                    contactType = ContactType.Fax;
                }

                long? phoneZone = null;
                int? phoneFormat = null;
                var contactValue = contactElement.Attribute(ContactValueAttributeName).Value;

                if (contactType == ContactType.Phone || contactType == ContactType.Fax)
                {
                    var phoneZoneAttribute = contactElement.Attribute(ZoneCodeAttributeName);
                    var phoneFormatAttribute = contactElement.Attribute(FormatCodeAttributeName);
                    phoneZone = (long?)phoneZoneAttribute;
                    phoneFormat = (int?)phoneFormatAttribute;
                }

                contacts.Add(new ContactDto
                    {
                        SortingPosition = sortingPositionCounter,
                        Contact = contactValue,
                        ContactType = contactType,
                        FormatCode = phoneFormat,
                        PhoneZoneCode = phoneZone
                    });
            }

            return contacts;
        }

        private static string ParseSchedule(XContainer scheduleElement)
        {
            const int DaysOfWeekNumber = 7;

            if (scheduleElement == null)
            {
                return null;
            }

            var result = new ScheduleDto();
            var dayScheduleElements = scheduleElement.Elements(DayElementName).ToArray();

            for (int i = 0; i < DaysOfWeekNumber; i++)
            {
                var dayScheduleElement = dayScheduleElements[i];
                var daySchedule = new DayScheduleDto
                    {
                        Label = (DayLabel)Enum.Parse(typeof(DayLabel), dayScheduleElement.Attribute(DayLabelAttributeName).Value, true)
                    };

                if (dayScheduleElement.Attribute(WorkDayBeginTimeAttributeName) != null)
                {
                    daySchedule.From = TimeSpan.Parse(dayScheduleElement.Attribute(WorkDayBeginTimeAttributeName).Value);
                    daySchedule.To = TimeSpan.Parse(dayScheduleElement.Attribute(WorkDayEndTimeAttributeName).Value);
                }

                var breakElement = dayScheduleElement.Element(BreakElementName);
                if (breakElement != null)
                {
                    daySchedule.Break = new BreakDto
                        {
                            From = TimeSpan.Parse(breakElement.Attribute(BreakBeginTimeAttributeName).Value),
                            To = TimeSpan.Parse(breakElement.Attribute(BreakEndTimeAttributeName).Value)
                        };
                }

                result.Schedule[i] = daySchedule;
            }

            return Format(result);
        }

        private static IEnumerable<ImportCategoryFirmAddressDto> ParseRubrics(XContainer rubricsElement, long cardCode)
        {
            if (rubricsElement == null)
            {
                return Enumerable.Empty<ImportCategoryFirmAddressDto>();
            }

            var rubrics = new List<ImportCategoryFirmAddressDto>();

            var sortingPositionCounter = 0;
            foreach (var rubricElement in rubricsElement.Elements())
            {
                sortingPositionCounter++;

                rubrics.Add(new ImportCategoryFirmAddressDto
                    {
                        SortingPosition = sortingPositionCounter,
                        CategoryId = (int)rubricElement.Attribute(RubricCodedAttributeName),
                        IsPrimary = rubricElement.Attribute(RubricIsPrimaryAttributeName) != null && (bool)rubricElement.Attribute(RubricIsPrimaryAttributeName),
                        FirmAddressId = cardCode
                    });
            }

            return rubrics;
        }

        private static IEnumerable<int> ParsePaymentMethodCodes(XContainer fieldsElement)
        {
            if (fieldsElement == null)
            {
                return Enumerable.Empty<int>();
            }

            return
                fieldsElement.Elements(ReferenceListFieldElementName)
                             .Where(x => x.Attribute(CodeAttributeName).Value == PaymentMethodCode &&
                                 (x.Attribute(IsHiddenAttributeName) == null || !(bool)x.Attribute(IsHiddenAttributeName)))
                             .Select(x => x.Element(ItemsElementName))
                             .SelectMany(x => x.Elements(ItemElementName))
                             .Select(x => (int)x.Attribute(CodeAttributeName))
                             .ToArray();
        }

        private static string ParsePoBox(XContainer fieldsElement)
        {
            if (fieldsElement == null)
            {
                return null;
            }

            return fieldsElement.Elements(TextFieldElementName)
                                .Where(x => x.Attribute(CodeAttributeName).Value == PoBoxCode &&
                                            (x.Attribute(IsHiddenAttributeName) == null || !(bool)x.Attribute(IsHiddenAttributeName)))
                                .Select(x => x.Attribute(ValueAttributeName).Value)
                                .SingleOrDefault();
        }

        private static string Format(ScheduleDto schedule)
        {
            if (schedule == null)
            {
                return null;
            }

            return string.Join(string.Empty, schedule.Schedule.Select(Format));
        }

        private static string Format(DayScheduleDto daySchedule)
        {
            const string NewFormatPrefix = "{NF}";
            const string Separator = "|";
            const string TimeFormat = @"hh\:mm";

            var workTime = daySchedule.From != null
                               ? daySchedule.From.Value.ToString(TimeFormat) + " - " + daySchedule.To.Value.ToString(TimeFormat)
                               : string.Empty;
            var breakTime = daySchedule.Break != null
                                ? Separator + daySchedule.Break.From.ToString(TimeFormat) + " - " + daySchedule.Break.To.ToString(TimeFormat)
                                : string.Empty;
            return NewFormatPrefix + daySchedule.Label + Separator + workTime + breakTime + ";";
        }
    }
}