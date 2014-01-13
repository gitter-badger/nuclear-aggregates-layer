using System;
using System.DirectoryServices;
using System.Text;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.AD
{
    public sealed class GetUserInfoFromAdService : IGetUserInfoService, IDisposable
    {
        #region Поля
        private const string UserSearchTemlate = "(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))";
        #endregion

        #region Свойства
        private DirectoryEntry Root { get; set; }
        private DirectorySearcher UserSearcher { get; set; }
        #endregion

        #region Конструкторы
        public GetUserInfoFromAdService(IGetUserInfoFromAdSettings settings)
        {
            #region Проверка входных данных
            if (string.IsNullOrWhiteSpace(settings.ADConnectionDomainName))
            {
                throw new ArgumentException("Settings param DomainName has invalid value");
            }

            if (string.IsNullOrWhiteSpace(settings.ADConnectionLogin))
            {
                throw new ArgumentException("Settings param Login has invalid value");
            } 
            #endregion
            
            Root = GetRootEntry(settings.ADConnectionDomainName, settings.ADConnectionLogin, settings.ADConnectionPassword);
            UserSearcher = GetSearcher(Root);
        }
        #endregion

        #region Методы

        private static DirectorySearcher GetSearcher(DirectoryEntry root)
        {
            #region Проверка входных данных
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            #endregion

            var result = new DirectorySearcher(root);
            result.PropertiesToLoad.Add("mail"); // Email
            result.PropertiesToLoad.Add("givenName"); // Имя
            result.PropertiesToLoad.Add("sn"); // Фамилия
            result.PropertiesToLoad.Add("displayName"); // Фамилия
            result.PropertiesToLoad.Add("title"); // Должность
            result.PropertiesToLoad.Add("telephoneNumber"); // Рабочий телефон
            result.PropertiesToLoad.Add("co"); // Страна
            result.PropertiesToLoad.Add("department"); // Отдел
            result.PropertiesToLoad.Add("streetAddress"); // Улица офиса
            result.PropertiesToLoad.Add("manager"); // Руководитель
            result.PropertiesToLoad.Add("Birthday"); // День рождения
            result.PropertiesToLoad.Add("Gender"); // Пол
            result.PropertiesToLoad.Add("mobile"); // Номер мобильного телефона
            result.PropertiesToLoad.Add("postalCode"); // Почтовый индекс
            return result;
        }

        private static DirectoryEntry GetRootEntry(string domainName, string login, string password)
        {
            string[] domainParts = domainName.Split('.');

            #region Проверка входных данных
            if (domainParts.Length < 2)
            {
                throw new ArgumentException(BLResources.WrongDomainName, "domainName");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }
            #endregion

            StringBuilder domainBuilder = new StringBuilder(@"LDAP://").Append(domainName).Append("/");
            domainBuilder = domainBuilder.Append("DC=").Append(domainParts[0]);
            for (var i = 1; i < domainParts.Length; i++)
            {
                domainBuilder = domainBuilder.Append(",DC=").Append(domainParts[i]);
            }

            var result = new DirectoryEntry(domainBuilder.ToString(), login, password);
            return result;
        }

        /// <summary>
        /// Получает информацию о пользователе из Active Directory по его имени учетной записи в домене
        /// </summary>
        /// <param name="domainUserName">Учетная запись пользователя в домене</param>
        /// <returns></returns>
        ADUserProfile IGetUserInfoService.GetInfo(string domainUserName)
        {
            #region Проверка входных данных
            if (string.IsNullOrWhiteSpace(domainUserName))
            {
                throw new ArgumentNullException("domainUserName");
            } 
            #endregion

            return GetInfoFromAD(Root, UserSearcher, domainUserName);
        }

        /// <summary>
        /// Получает информацию о пользователе из Active Directory по его имени учетной записи в домене
        /// </summary>
        /// <param name="domainName">Имя домена</param>
        /// <param name="login">Логин для подключения к домену</param>
        /// <param name="password">Пароль для подключения к домену</param>
        /// <param name="domainUserName">Учетная запись пользователя в домене</param>
        /// <returns></returns>
        public ADUserProfile GetInfo(string domainName, string login, string password, string domainUserName)
        {
            #region Проверка входных данных
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentNullException("domainName");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }

            if (string.IsNullOrWhiteSpace(domainUserName))
            {
                throw new ArgumentNullException("domainUserName");
            }
            #endregion

            var root = GetRootEntry(domainName, login, password);
            var searcher = GetSearcher(root);

            return GetInfoFromAD(root, searcher, domainUserName);
        }

        private ADUserProfile GetInfoFromAD(DirectoryEntry root, DirectorySearcher searcher, string domainUserName)
        {
            #region Проверка входных данных
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (searcher == null)
            {
                throw new ArgumentNullException("searcher");
            }

            if (string.IsNullOrWhiteSpace(domainUserName))
            {
                throw new ArgumentNullException("domainUserName");
            }
            #endregion

            searcher.Filter = string.Format(UserSearchTemlate, domainUserName);
            SearchResult searchResultresult = searcher.FindOne();
            if (searchResultresult == null)
            {
                throw new NotificationException(string.Format(BLResources.UserNotFound, domainUserName));
            }

            DirectoryEntry user = searchResultresult.GetDirectoryEntry();

            var result = new ADUserProfile
                {
                    Email = GetPropertyOrDefault(user, "mail", val => val.ToString(), string.Empty),
                    DisplayName = GetPropertyOrDefault(user, "displayName", val => val.ToString(), string.Empty),
                    FirstName = GetPropertyOrDefault(user, "givenName", val => val.ToString(), string.Empty),
                    LastName = GetPropertyOrDefault(user, "sn", val => val.ToString(), string.Empty),
                    Mobile = GetPropertyOrDefault(user, "mobile", val => val.ToString(), string.Empty),
                    Phone = GetPropertyOrDefault(user, "telephoneNumber", val => val.ToString(), string.Empty),
                    Position = GetPropertyOrDefault(user, "title", val => val.ToString(), string.Empty),
                    Company = GetPropertyOrDefault(user, "company", val => val.ToString(), string.Empty)
                };

            var address = new StringBuilder();
            if (user.Properties["postalCode"].Value != null)
            {
                address.AppendFormat("{0}, ", user.Properties["postalCode"].Value);
            }

            if (user.Properties["co"].Value != null)
            {
                address.AppendFormat("{0}, ", user.Properties["co"].Value);
            }

            if (user.Properties["st"].Value != null)
            {
                address.AppendFormat("{0}, ", user.Properties["st"].Value);
            }

            if (user.Properties["streetAddress"].Value != null)
            {
                address.AppendFormat("{0}", user.Properties["streetAddress"].Value);
            }

            if (user.Properties["Gender"].Value != null)
            {
                switch (user.Properties["Gender"].Value.ToString().ToUpper())
                {
                    case "F":
                        result.Gender = Gender.Female;
                        break;
                    case "M":
                        result.Gender = Gender.Male;
                        break;
                    default:
                        result.Gender = Gender.None;
                        break;
                }
            }

            result.Address = address.ToString();
            DateTime birthday;
            if (user.Properties["Birthday"].Value != null && DateTime.TryParse(user.Properties["Birthday"].Value.ToString(), out birthday))
            {
                result.BirthDay = birthday;
            }

            result.PlanetURL = "https://planeta.2gis.local/profile/" + domainUserName;
            return result;
        }

        private static TResult GetPropertyOrDefault<TResult>(
            DirectoryEntry directoryEntry, 
            string propertyName,  
            Func<object, TResult> convertValue, 
            TResult defaultValue = default(TResult))
        {
            var property = directoryEntry.Properties[propertyName];
            if (property == null)
            {
                return defaultValue;
            }

            if (property.Value == null)
            {
                return defaultValue;
            }

            return convertValue(property.Value);
        }

        /// <summary>
        /// Получает информацию о пользователе из Active Directory по его имени учетной записи в домене
        /// </summary>
        /// <param name="domainUserName">Учетная запись пользователя в домене</param>
        /// <param name="profile"></param>
        /// <returns></returns>
        bool IGetUserInfoService.TryGetInfo(string domainUserName, out ADUserProfile profile)
        {
            try
            {
                var self = (IGetUserInfoService) this;
                profile = self.GetInfo(domainUserName);
                return true;
            }
            catch
            {
                profile = null;
                return false;
            }
        }

        /// <summary>
        /// Получает информацию о пользователе из Active Directory по его имени учетной записи в домене
        /// </summary>
        /// <param name="domainName">Имя домена</param>
        /// <param name="login">Логин для подключения к домену</param>
        /// <param name="password">Пароль для подключения к домену</param>
        /// <param name="domainUserName">Учетная запись пользователя в домене</param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool TryGetInfo(string domainName, string login, string password, string domainUserName, out ADUserProfile profile)
        {
            try
            {
                profile = GetInfo(domainName, login, password, domainUserName);
                return true;
            }
            catch
            {
                profile = null;
                return false;
            }
        }

        public void Dispose()
        {
            CloseHandles();
            GC.SuppressFinalize(this);
        }

        private void CloseHandles()
        {
            if (Root != null)
            {
                Root.Close();
                Root = null;
            }

            if (UserSearcher != null)
            {
                UserSearcher.Dispose();
                UserSearcher = null;
            }
        }

        ~GetUserInfoFromAdService()
        {
            CloseHandles();
        }
        #endregion
    }
}
