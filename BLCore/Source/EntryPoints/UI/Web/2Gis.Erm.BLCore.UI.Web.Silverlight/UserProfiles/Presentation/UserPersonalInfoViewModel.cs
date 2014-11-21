using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.MVVM;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public class UserPersonalInfoViewModel : ViewModelBase, IUserPersonalInfoViewModel
    {
        private readonly long _userCode;
        private readonly IUserProfileDataProvider _userProfileDataProvider;

        public UserPersonalInfoViewModel(long userCode, IUserProfileDataProvider userProfileDataProvider)
        {
            _userCode = userCode;
            _userProfileDataProvider = userProfileDataProvider;

            var personalInfo = _userProfileDataProvider.GetPersonalInfo(userCode);
            if (personalInfo != null)
            {
                Address = personalInfo.Address;
                BirthDay = personalInfo.BirthDay;
                Company = personalInfo.Company;
                DisplayName = personalInfo.DisplayName;
                Email = personalInfo.Email;
                FirstName = personalInfo.FirstName;
                Gender = personalInfo.Gender;
                LastName = personalInfo.LastName;
                Manager = personalInfo.Manager;
                Mobile = personalInfo.Mobile;
                Phone = personalInfo.Phone;
                PlanetURL = personalInfo.PlanetURL;
                Position = personalInfo.Position;
            }
        }
        
        #region Implementation of IUserPersonalInfoViewModel

        public UserPersonalInfoDto PersonalInfo
        {
            get
            {
                return new UserPersonalInfoDto
                {
                    Address = Address,
                    BirthDay = BirthDay,
                    Company = Company,
                    DisplayName = DisplayName,
                    Email = Email,
                    FirstName = FirstName,
                    Gender = Gender,
                    LastName = LastName,
                    Manager = Manager,
                    Mobile = Mobile,
                    Phone = Phone,
                    PlanetURL = PlanetURL,
                    Position = Position    
                };
            }
        }

        private string _firstName;

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    RaisePropertyChanged(() => FirstName);
                }
            }
        }

        private string _lastName;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    RaisePropertyChanged(() => LastName);
                }
            }
        }

        private string _displayName;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    RaisePropertyChanged(() => DisplayName);
                }
            }
        }

        private string _company;
        /// <summary>
        /// Компания (филиал)
        /// </summary>
        public string Company
        {
            get
            {
                return _company;
            }
            set
            {
                if (_company != value)
                {
                    _company = value;
                    RaisePropertyChanged(() => Company);
                }
            }
        }

        private string _position;
        /// <summary>
        /// Должность
        /// </summary>
        public string Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged(() => Position);
                }
            }
        }

        private string _email;
        /// <summary>
        /// Адрес рабочей электронной почты
        /// </summary>
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    RaisePropertyChanged(() => Email);
                }
            }
        }

        private string _mobile;
        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        public string Mobile
        {
            get
            {
                return _mobile;
            }
            set
            {
                if (_mobile != value)
                {
                    _mobile = value;
                    RaisePropertyChanged(() => Mobile);
                }
            }
        }

        private string _phone;
        /// <summary>
        /// Рабочий телефон
        /// </summary>
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    RaisePropertyChanged(() => Phone);
                }
            }
        }

        private string _address;
        /// <summary>
        /// Местонахождения
        /// </summary>
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    RaisePropertyChanged(() => Address);
                }
            }
        }

        private string _planetURL;
        /// <summary>
        /// Адрес профиля на планете.
        /// </summary>
        public string PlanetURL
        {
            get
            {
                return _planetURL;
            }
            set
            {
                if (_planetURL != value)
                {
                    _planetURL = value;
                    RaisePropertyChanged(() => PlanetURL);
                }
            }
        }
        private string _manager;
        /// <summary>
        /// Руководитель
        /// </summary>
        public string Manager
        {
            get
            {
                return _manager;
            }
            set
            {
                if (_manager != value)
                {
                    _manager = value;
                    RaisePropertyChanged(() => Manager);
                }
            }
        }

        private DateTime? _birthDay;
        /// <summary>
        /// День рождения
        /// </summary>
        public DateTime? BirthDay
        {
            get
            {
                return _birthDay;
            }
            set
            {
                if (_birthDay != value)
                {
                    _birthDay = value;
                    RaisePropertyChanged(() => BirthDay);
                }
            }
        }

        private UserGender _gender;
        /// <summary>
        /// Пол
        /// </summary>
        public UserGender Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    RaisePropertyChanged(() => Gender);
                }
            }
        }

        #endregion
    }
}