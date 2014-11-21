using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public interface IUserPersonalInfoViewModel
    {
        /// <summary>
        /// Имя
        /// </summary>
        string FirstName { get;  }
        /// <summary>
        /// Фамилия
        /// </summary>
        string LastName { get;  }
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        string DisplayName { get;  }
        /// <summary>
        /// Компания (филиал)
        /// </summary>
        string Company { get;  }
        /// <summary>
        /// Должность
        /// </summary>
        string Position { get;  }
        /// <summary>
        /// Адрес рабочей электронной почты
        /// </summary>
        string Email { get;  }
        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        string Mobile { get;  }
        /// <summary>
        /// Рабочий телефон
        /// </summary>
        string Phone { get;  }
        /// <summary>
        /// Местонахождения
        /// </summary>
        string Address { get;  }
        /// <summary>
        /// Адрес профиля на планете.
        /// </summary>
        string PlanetURL { get;  }
        /// <summary>
        /// Руководитель
        /// </summary>
        string Manager { get;  }
        /// <summary>
        /// День рождения
        /// </summary>
        DateTime? BirthDay { get;  }
        /// <summary>
        /// Пол
        /// </summary>
        UserGender Gender { get;  }

        UserPersonalInfoDto PersonalInfo { get; }
    }
}
