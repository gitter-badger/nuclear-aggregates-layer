using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD
{
    /// <summary>
    /// Представляет данные профиля пользователя
    /// </summary>
    public sealed class ADUserProfile
    {
        #region Свойства
        /// <summary>
        /// Имя
        /// </summary>
        public String FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Фамилия
        /// </summary>
        public String LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public String DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Компания (филиал)
        /// </summary>
        public String Company
        {
            get;
            set;
        }

        /// <summary>
        /// Должность
        /// </summary>
        public String Position
        {
            get;
            set;
        }

        /// <summary>
        /// Адрес рабочей электронной почты
        /// </summary>
        public String Email
        {
            get;
            set;
        }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        public String Mobile
        {
            get;
            set;
        }

        /// <summary>
        /// Номер рабочего или внутреннего телефона 
        /// </summary>
        public String Phone
        {
            get;
            set;
        }

        /// <summary>
        /// Адрес
        /// </summary>
        public String Address
        {
            get;
            set;
        }

        /// <summary>
        /// Адрес профиля на планете.
        /// </summary>
        public String PlanetURL
        {
            get;
            set;
        }

        /// <summary>
        /// Руководитель
        /// </summary>
        public String Manager
        {
            get;
            set;
        }

        /// <summary>
        /// День рождения
        /// </summary>
        public DateTime? BirthDay
        {
            get;
            set;
        }

        /// <summary>
        /// Пол
        /// </summary>
        public Gender Gender
        {
            get;
            set;
        }
        #endregion
    }
}