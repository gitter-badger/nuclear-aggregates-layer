using System;
using System.Globalization;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
{
    [Serializable]
    public class UserLocaleInfo
    {
        public const string UserLocaleInfoKey = "UserLocaleInfoKey";
        
        private readonly string _twoLetterISOLanguageName;
        private readonly string _cultureName;
        private readonly NumberFormatInfoDto _numberFormatInfo;
        private readonly DateTimeFormatInfoDto _dateTimeFormatInfo;

        public UserLocaleInfo(string cultureName, string twoLetterISOLanguageName, NumberFormatInfo numberFormatInfo, DateTimeFormatInfo dateTimeFormatInfo, int timeOffsetInMinutes, string timeZoneId)
        {
            _cultureName = cultureName;
            _twoLetterISOLanguageName = twoLetterISOLanguageName;
            _numberFormatInfo = numberFormatInfo.ToDto();
            _dateTimeFormatInfo = dateTimeFormatInfo.ToDto(timeOffsetInMinutes, timeZoneId);
        }

        public DateTimeFormatInfoDto DateTimeFormatInfo
        {
            get { return _dateTimeFormatInfo; }
        }

        public NumberFormatInfoDto NumberFormatInfo
        {
            get { return _numberFormatInfo; }
        }

        public string TwoLetterISOLanguageName
        {
            get { return _twoLetterISOLanguageName; }
        }

        public string CultureName
        {
            get { return _cultureName; }
        }
    }
}