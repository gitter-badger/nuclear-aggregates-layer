using System;
using System.Configuration;
using System.Data.Common;
using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.Migration.Runner;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class ConnectionStringsKnower
    {
        private const string EnvironmentNumRegexTemplate = @"^[a-zA-Z]+(?<EnvironmentNumber>\d+)?$";
        private const string DefaultConnectionStringKey = "Erm";
        private readonly ConnectionStringSettingsCollection _connectionStrings;
        private readonly bool _useCrmConnection;

        public ConnectionStringsKnower(ConnectionStringSettingsCollection connectionStrings, bool useCrmConnection)
        {
            if (connectionStrings == null)
            {
                throw new ArgumentNullException("connectionStrings");
            }

            if (connectionStrings[DefaultConnectionStringKey] == null)
            {
                throw new ArgumentException("Connection strings collection doesn't contain " + DefaultConnectionStringKey + " key");
            }

            _connectionStrings = connectionStrings;
            _useCrmConnection = useCrmConnection;
        }

        public static string GetEnvironmentSuffix(string connectionString)
        {
            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var regex = new Regex(EnvironmentNumRegexTemplate);
            var match = regex.Match((string)connectionStringBuilder["initial catalog"]);
            if (match.Success)
            {
                // имя БД корректное, проверим есть ли номер environment'a в имени БД
                var group = match.Groups["EnvironmentNumber"];
                return group.Value; // если нет группы EnvironmentNumber - то вернет String.Empty
            }

            return string.Empty;
        }

        public string GetConnectionString(ErmConnectionStringKey key = ErmConnectionStringKey.Default)
        {
            // Если запрашивается подключение к базе crm, то изготовим его с помощью 
            // такой то матери, порции костылей и строки подключения к erm.
            var connectionStringKeyForRequest = key == ErmConnectionStringKey.Crm
                                                    ? ErmConnectionStringKey.Default
                                                    : key;

            var connectionStringBuilder = new DbConnectionStringBuilder
                {
                    ConnectionString = GetConnectionStringInternal(connectionStringKeyForRequest)
                };

            if (key == ErmConnectionStringKey.Crm)
            {
                // Для подключения к Crm используем номер среды, использованный в строке подключения БД Erm
                // А для машин разработчиков используем несуществующий номер окружения.
                var currentEnvironmentSuffix = _useCrmConnection 
                    ? GetEnvironmentSuffix(GetConnectionStringInternal(ErmConnectionStringKey.Erm)) 
                    : "99";

                connectionStringBuilder["initial catalog"] = EnvironmentUtil.GetMsCrmDatabaseName(currentEnvironmentSuffix);
            }

            return connectionStringBuilder.ConnectionString;
        }

        private string GetConnectionStringInternal(ErmConnectionStringKey key)
        {
            switch (key)
            {
                case ErmConnectionStringKey.Logging:
                    return _connectionStrings["ErmLogging"].ConnectionString;
                case ErmConnectionStringKey.Erm:
                    return _connectionStrings["Erm"].ConnectionString;
                case ErmConnectionStringKey.Crm:
                    return null;
                case ErmConnectionStringKey.CrmWebService:
                    return _connectionStrings["CrmConnection"].ConnectionString;
            }

            return null;
        }
    }
}
