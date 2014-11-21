using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.MVVM;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public class UserLocaleInfoViewModel : ViewModelBase, IUserLocaleInfoViewModel
    {
        public class SampleLocaleData
        {
            public SampleLocaleData()
            {
                Number = string.Empty;
                Currency = string.Empty;
                Time = string.Empty;
                DateShort = string.Empty;
                DateLong = string.Empty;
            }

            public SampleLocaleData(decimal sampleNumber, DateTime sampleDateTime, CultureInfo usingCulture)
            {
                Number = sampleNumber.ToString("F", usingCulture.NumberFormat);
                Currency = sampleNumber.ToString("C", usingCulture.NumberFormat);
                Time = sampleDateTime.ToString("t", usingCulture.DateTimeFormat);
                DateShort = sampleDateTime.ToString("d", usingCulture.DateTimeFormat);
                DateLong = sampleDateTime.ToString("D", usingCulture.DateTimeFormat);
            }

            public string Number { get; set; }
            public string Currency { get; set; }
            public string Time { get; set; }
            public string DateShort { get; set; }
            public string DateLong { get; set; }
        }

        private readonly long _userCode;
        private readonly IUserProfileDataProvider _userProfileDataProvider;

        private readonly decimal _sampleNumber;
        private readonly DateTime _sampleDateTime;

        public UserLocaleInfoViewModel(long userCode, IUserProfileDataProvider userProfileDataProvider)
        {
            _sampleNumber = 123456789m;
            _sampleDateTime = DateTime.Now;
            _userCode = userCode;

            _userProfileDataProvider = userProfileDataProvider;
            var supportedSettings = _userProfileDataProvider.SupportedLocalSettings;
            var localeSettings = _userProfileDataProvider.GetLocalSettings(userCode);

            SupportedCultures = supportedSettings.SupportedCultures;
            SupportedTimeZones = supportedSettings.SupportedTimeZones;

            if (localeSettings != null)
            {
                SelectedCulture = supportedSettings.SupportedCultures.FirstOrDefault(с => string.CompareOrdinal(с.Name, localeSettings.CurrentCultureInfoName) == 0);
                SelectedTimeZone = supportedSettings.SupportedTimeZones.FirstOrDefault(t => t.Id == localeSettings.CurrentTimeZoneId);
            }
        }

        #region Implementation of IUserLocaleInfoViewModel

        private CultureInfoDto[] _supportedCultures;
        /// <summary>
        /// Свойство supportedCultures использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public CultureInfoDto[] SupportedCultures
        {
            get
            {
                return _supportedCultures;
            }
            private set
            {
                if (_supportedCultures != value)
                {
                    _supportedCultures = value;
                    RaisePropertyChanged(() => SupportedCultures);
                }
            }
        }

        private CultureInfo _selectedCultureInstance;

        private CultureInfoDto _selectedCulture;
        /// <summary>
        /// Свойство selectedCulture использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public CultureInfoDto SelectedCulture
        {
            get
            {
                return _selectedCulture;
            }
            set
            {
                if (_selectedCulture != value)
                {
                    _selectedCulture = value;
                    RaisePropertyChanged(() => SelectedCulture);
                    if (value != null)
                    {
                        _selectedCultureInstance = new CultureInfo(value.Name);
                        SampleCultureData = new SampleLocaleData(_sampleNumber, _sampleDateTime, _selectedCultureInstance);
                    }
                    else
                    {
                        _selectedCultureInstance = null;
                        SampleCultureData = new SampleLocaleData();
                    }
                }
            }
        }

        private TimeZoneDto[] _supportedTimeZones;
        /// <summary>
        /// Свойство supportedTimeZones использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public TimeZoneDto[] SupportedTimeZones
        {
            get
            {
                return _supportedTimeZones;
            }
            private set
            {
                if (_supportedTimeZones != value)
                {
                    _supportedTimeZones = value;
                    RaisePropertyChanged(() => SupportedTimeZones);
                }
            }
        }

        private TimeZoneDto _selectedTimeZone;
        /// <summary>
        /// Свойство selectedTimeZone использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public TimeZoneDto SelectedTimeZone
        {
            get
            {
                return _selectedTimeZone;
            }
            set
            {
                if (_selectedTimeZone != value)
                {
                    _selectedTimeZone = value;
                    RaisePropertyChanged(() => SelectedTimeZone);
                }
            }
        }

        private SampleLocaleData _sampleCultureData;
        /// <summary>
        /// Свойство sampleCultureData использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public SampleLocaleData SampleCultureData
        {
            get
            {
                return _sampleCultureData;
            }
            private set
            {
                if (_sampleCultureData != value)
                {
                    _sampleCultureData = value;
                    RaisePropertyChanged(() => SampleCultureData);
                }
            }
        }
        public LocalSettingsDto LocalSettings
        {
            get
            {
                if (SelectedCulture == null || SelectedTimeZone == null)
                {
                    return null;
                }

                return new LocalSettingsDto
                {
                    CurrentCultureInfoName = SelectedCulture.Name,
                    CurrentTimeZoneId = SelectedTimeZone.Id
                };
            }
        }

        #endregion
    }
}