using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class ConnectionStringsKnower
    {
        //private const string EnvironmentNumRegexTemplate = @"^[a-zA-Z]+(?<EnvironmentNumber>\d+)?$";
        private readonly ConnectionStringSettingsCollection _connectionStrings;

        public ConnectionStringsKnower(ConnectionStringSettingsCollection connectionStrings)
        {
            if (connectionStrings == null)
            {
                throw new ArgumentNullException("connectionStrings");
            }

            _connectionStrings = connectionStrings;
        }

        //public static string GetEnvironmentSuffix(string connectionString)
        //{
        //    var connectionStringBuilder = new DbConnectionStringBuilder
        //    {
        //        ConnectionString = connectionString
        //    };

        //    var regex = new Regex(EnvironmentNumRegexTemplate);
        //    var match = regex.Match((string)connectionStringBuilder["initial catalog"]);
        //    if (match.Success)
        //    {
        //        // имя БД корректное, проверим есть ли номер environment'a в имени БД
        //        var group = match.Groups["EnvironmentNumber"];
        //        return group.Value; // если нет группы EnvironmentNumber - то вернет String.Empty
        //    }

        //    return string.Empty;
        //}

        public bool TryGetDatabaseName(ErmConnectionStringKey key, out string databaseName)
        {
            switch (key)
            {
                case ErmConnectionStringKey.Erm:
                case ErmConnectionStringKey.Logging:
                    {
                        string connectionString;
                        if (!TryGetConnectionStringInternal(key.ToString(), out connectionString))
                        {
                            throw new ArgumentException(string.Format("Cannot find connection string {0}", key));
                        }

                        var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
                        databaseName = (string)connectionStringBuilder["Initial Catalog"];
                        return true;
                    }

                case ErmConnectionStringKey.CrmConnection:
                case ErmConnectionStringKey.CrmDatabase:
                    {
                        string connectionString;
                        if (!TryGetConnectionStringInternal("CrmConnection", out connectionString) || string.IsNullOrEmpty(connectionString))
                        {
                            databaseName = null;
                            return false;
                        }

                        var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
                        var organizationUrl = (string)connectionStringBuilder["server"];
                        var uriBuilder = new UriBuilder(organizationUrl);
                        var organizationName = uriBuilder.Path.Trim('/');
                        databaseName = organizationName + "_MSCRM";
                        return true;
                    }

                default:
                    throw new ArgumentException("key");
            }
        }

        public bool TryGetConnectionString(ErmConnectionStringKey key, out string connectionString)
        {
            switch (key)
            {
                case ErmConnectionStringKey.Erm:
                    {
                        return TryGetConnectionStringInternal("Erm", out connectionString);
                    }

                case ErmConnectionStringKey.Logging:
                    {
                        return TryGetConnectionStringInternal("Logging", out connectionString);
                    }

                case ErmConnectionStringKey.CrmConnection:
                    {
                        return TryGetConnectionStringInternal("CrmConnection", out connectionString);
                    }

                case ErmConnectionStringKey.CrmDatabase:
                    {
                        string crmDatabaseName;
                        if (!TryGetDatabaseName(key, out crmDatabaseName))
                        {
                            connectionString = null;
                            return false;
                        }

                        if (!TryGetConnectionStringInternal("Erm", out connectionString))
                        {
                            connectionString = null;
                            return false;
                        }

                        connectionString = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = crmDatabaseName }.ConnectionString;
                        return true;
                    }
            }

            connectionString = null;
            return false;
        }

        private bool TryGetConnectionStringInternal(string connectionStringNameAlias, out string connectionString)
        {
            var connectionStringSettings = _connectionStrings[connectionStringNameAlias];
            if (connectionStringSettings == null)
            {
                connectionString = null;
                return false;
            }

            connectionString = connectionStringSettings.ConnectionString;
            return true;
        }
    }
}
