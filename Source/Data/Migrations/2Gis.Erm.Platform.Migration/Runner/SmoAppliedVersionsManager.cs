using System;
using System.Data;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.Platform.Migration.Runner
{
    /// <summary>
    /// Класс для записи\считывания примененных миграций в\из БД на основе SqlServer.Smo.
    /// </summary>
    public class SmoAppliedVersionsManager : IAppliedVersionsManager
    {
        private static readonly VersionTableMetaData DefaultVersionTableMetaData = new VersionTableMetaData("Shared", "Migrations", "Version");

        private readonly MigrationContextManager _contextManager;
        private readonly VersionTableMetaData _versionTableMetaData;
        private AppliedVersionsInfo _appliedVersionsInfo;

        public SmoAppliedVersionsManager(MigrationContextManager contextManager, VersionTableMetaData versionTableMetaData = null)
        {
            _contextManager = contextManager;
            _versionTableMetaData = versionTableMetaData ?? DefaultVersionTableMetaData;
            LoadVersionInfo();
        }

        public AppliedVersionsInfo AppliedVersionsInfo
        {
            get
            {
                return _appliedVersionsInfo;
            }
        }

        public void LoadVersionInfo()
        {
            _appliedVersionsInfo = new AppliedVersionsInfo();

            using (var context = _contextManager.GetContext())
            {
                var versionTable = context.Database.GetTable(_versionTableMetaData.TableName);

                if (versionTable == null)
                {
                    return;
                }

                var sqlCommand = string.Format("SELECT {0} FROM {1}", _versionTableMetaData.ColumnName, _versionTableMetaData.TableName);

                var dataSet = context.Connection.ExecuteWithResults(sqlCommand);
                if (dataSet != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        _appliedVersionsInfo.AddAppliedMigration(long.Parse(row[0].ToString()));
                    }
                }
            }
        }

        public void SaveVersionInfo(long version)
        {
            var sqlCommand = string.Format("INSERT INTO {0} ({1}, DateApplied) VALUES ({2}, '{3}');\nGO\n", 
                _versionTableMetaData.TableName, 
                _versionTableMetaData.ColumnName, 
                version,
                DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"));

            using (var context = _contextManager.GetContext())
            {
                context.Connection.ExecuteNonQuery(sqlCommand);
            }
        }

        public void DeleteVersion(long version)
        {
            var sqlCommand = string.Format("DELETE FROM {0} WHERE [{1}] = {2};\nGO\n", _versionTableMetaData.TableName, _versionTableMetaData.ColumnName, version);

            using (var context = _contextManager.GetContext())
            {
                context.Connection.ExecuteNonQuery(sqlCommand);
            }
        }
    }
}