using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public sealed class SearchSettings : ISearchSettings, IEnvironmentIsolationSettings, IConnectionStringSettingsHost
    {
        private readonly string _targetEnvironmentName;
        private ConnectionStringsSettingsAspect _connectionStringsSettingsAspect;

        public SearchSettings()
        {
            var entryPointName = ConfigFileSetting.String.Required("EntryPointName").Value;
            _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName").Value;

            Init(entryPointName, _targetEnvironmentName);
        }

        public SearchSettings(string entryPointName, string targetEnvironmentName)
        {
            _targetEnvironmentName = targetEnvironmentName;
            Init(entryPointName, targetEnvironmentName);
        }

        public string Host { get; private set; }
        public Protocol Protocol { get; private set; }
        public int HttpPort { get; private set; }
        public int ThriftPort { get; private set; }

        public int BatchSize { get; private set; }
        public string TargetEnvironmentName
        {
            get
            {
                return _targetEnvironmentName;
            }
        }

        public ConnectionStringsSettingsAspect ConnectionStrings
        {
            get
            {
                return _connectionStringsSettingsAspect;
            }
        }

        // TODO: копипаст из ErmEnvironmentsSettingsAspect, убрать этот хак при мердже
        private static IDictionary<string, string> GetConnectionStringsMap(string entryPointName, string environmentName)
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                {
                    ExeConfigFilename = ErmEnvironmentsSettingsLoader.DefaultEnvironmentsConfigFullPath,
                },
                ConfigurationUserLevel.None);

            var targetConfigSettings = (ErmEnvironmentsDescriptionsConfigurationSection)config.GetSection("ermEnvironmentsSettings");
            var targetEnvironment = targetConfigSettings.ErmEnvironments.Cast<ErmEnvironmentElement>()
                                                                           .SingleOrDefault(x => x.Name == environmentName);

            if (targetEnvironment == null)
            {
                return null;
            }

            var entryPointOverride = targetEnvironment.EntryPointsOverrides.Cast<EntryPointOverrideElement>().SingleOrDefault(x => x.EntryPointName == entryPointName);

            var connectionStrings = targetEnvironment.ConnectionStrings.Cast<ConnectionStringSettings>();
            var connectionStringSettingsOverrides = entryPointOverride != null
                                      ? entryPointOverride.ConnectionStrings.Cast<ConnectionStringSettings>()
                                      : new ConnectionStringSettings[0];

            var specifiedConnectionStringsMap = connectionStrings.ToDictionary(c => c.Name, c => c.ConnectionString);
            foreach (var overridenStr in connectionStringSettingsOverrides)
            {
                specifiedConnectionStringsMap[overridenStr.Name] = overridenStr.ConnectionString;
            }

            return specifiedConnectionStringsMap;
        }

        private void Init(string entryPointName, string targetEnvironmentName)
        {
            var connectionStringsMap = GetConnectionStringsMap(entryPointName, targetEnvironmentName);
            _connectionStringsSettingsAspect = new ConnectionStringsSettingsAspect(new Dictionary<ConnectionStringName, string>
            {
                { ConnectionStringName.Erm, connectionStringsMap["Erm"] },
            });
            var connectionstring = connectionStringsMap["ErmSearch"];

            InitFromConnectionString(connectionstring);
        }

        private void InitFromConnectionString(string connectionString)
        {
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Host = (string)connectionStringBuilder["Host"];
            Protocol = (Protocol)Enum.Parse(typeof(Protocol), (string)connectionStringBuilder["Protocol"], true);
            HttpPort = Convert.ToInt32(connectionStringBuilder["HttpPort"]);
            ThriftPort = Convert.ToInt32(connectionStringBuilder["ThriftPort"]);
            BatchSize = Convert.ToInt32(connectionStringBuilder["BatchSize"]);
        }
    }
}