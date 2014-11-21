using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards
{
    // COMMENT {all, 27.05.2014}: В случае отказа от хранимки импорта карточек на остальных инсталяциях, этот класс можно будет перенести в BLCore
    public sealed class PhoneNumbersFormatter : IPhoneNumbersFormatter
    {
        private readonly IFirmReadModel _firmReadModel;

        public PhoneNumbersFormatter(IFirmReadModel firmReadModel)
        {
            _firmReadModel = firmReadModel;
        }

        public void FormatPhoneNumbers(IEnumerable<ContactDto> phonesAndFaxes)
        {
            var contactDtos = phonesAndFaxes as ContactDto[] ?? phonesAndFaxes.ToArray();

            var unsupportedTypes = contactDtos
                .Where(x => x.ContactType != ContactType.Phone && x.ContactType != ContactType.Fax)
                .Select(x => x.ContactType.ToString())
                .Distinct()
                .ToArray();

            if (unsupportedTypes.Any())
            {
                throw new InvalidOperationException("Неподдерживаемые для форматирования типы контактов:" + string.Join(",", unsupportedTypes));
            }

            var phoneZoneCodes = contactDtos.Where(x => x.PhoneZoneCode.HasValue).Select(x => x.PhoneZoneCode.Value).Distinct().ToArray();
            var phoneFormatCodes = contactDtos.Where(x => x.FormatCode.HasValue).Select(x => x.FormatCode.Value).Distinct().ToArray();

            var phoneZones = _firmReadModel.GetCityPhoneZones(phoneZoneCodes);
            var codesForUnknownPhoneZones = phoneZoneCodes.Where(x => !phoneZones.ContainsKey(x)).ToArray();
            if (codesForUnknownPhoneZones.Any())
            {
                throw new InvalidOperationException("Не известны значения телефонных кодов со следующими идентификаторами:" +
                                                    string.Join(",", codesForUnknownPhoneZones.Select(x => x.ToString())));
            }

            var phoneFormats = _firmReadModel.GetPhoneFormats(phoneFormatCodes);
            var codesForUnknownPhoneFormats = phoneFormatCodes.Where(x => !phoneFormats.ContainsKey(x)).ToArray();
            if (codesForUnknownPhoneFormats.Any())
            {
                throw new InvalidOperationException("Не известны значения форматов телефонных номеров со следующими идентификаторами:" +
                                                    string.Join(",", codesForUnknownPhoneFormats.Select(x => x.ToString())));
            }

            foreach (var contact in contactDtos.Where(x => x.FormatCode.HasValue))
            {
                contact.Contact = FormatPhoneAndFax(contact.Contact,
                                                    phoneFormats[contact.FormatCode.Value],
                                                    contact.PhoneZoneCode.HasValue ? phoneZones[contact.PhoneZoneCode.Value] : null);
            }
        }

        private static string FormatPhoneAndFax(string phoneValue, string phoneFormat, string zoneValue)
        {
            var array = new char[phoneFormat.Length];

            var length = phoneValue.Length;
            for (var i = 0; i < phoneFormat.Length; i++)
            {
                var index = (phoneFormat.Length - i) - 1;
                if (char.IsDigit(phoneFormat[index]))
                {
                    array[index] = phoneValue[--length];
                }
                else
                {
                    array[index] = phoneFormat[index];
                }

                if (length == 0)
                {
                    break;
                }
            }

            var num4 = Array.LastIndexOf(array, '\0');
            phoneValue = new string(array, num4 + 1, (array.Length - num4) - 1);
            if (!string.IsNullOrEmpty(zoneValue))
            {
                phoneValue = string.Format("({0}) {1}", zoneValue, phoneValue);
            }

            return phoneValue;
        }
    }
}
