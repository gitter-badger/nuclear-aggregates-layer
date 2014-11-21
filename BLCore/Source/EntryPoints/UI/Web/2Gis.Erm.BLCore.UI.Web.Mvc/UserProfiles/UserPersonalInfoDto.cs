using System;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class UserPersonalInfoDto
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Компания (филиал)
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Адрес рабочей электронной почты
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Номер рабочего или внутреннего телефона 
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Адрес профиля на планете.
        /// </summary>
        public string PlanetURL { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public string Manager { get; set; }

         /// <summary>
        /// День рождения
        /// </summary>
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public UserGender Gender { get; set; }
    }
}