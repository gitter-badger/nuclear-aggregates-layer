using System;
using System.Globalization;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest;
using Nest.Resolvers;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public class ElasticAppliedVersionsManager : IAppliedVersionsManager
    {
        private const string IndexName = "metadata";
        private const string TypeName = "MigrationDoc";

        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;
        private readonly string _typeName;

        public ElasticAppliedVersionsManager(IElasticClient elasticClient, IElasticConnectionSettingsFactory elasticConnectionSettingsFactory)
        {
            _elasticClient = elasticClient;
            _indexName = elasticConnectionSettingsFactory.GetIsolatedIndexName(IndexName);
            _typeName = TypeName.MakePlural().ToLowerInvariant();
        }

        public AppliedVersionsInfo AppliedVersionsInfo { get; private set; }
        public void LoadVersionInfo()
        {
            AppliedVersionsInfo = new AppliedVersionsInfo();

            var mapping = _elasticClient.GetMapping(_indexName, _typeName);
            if (mapping == null)
            {
                return;
            }

            var response = _elasticClient.Search(x => x.Index(_indexName).Type(_typeName).MatchAll());
            if (!response.IsValid)
            {
                throw new InvalidOperationException("Error then reading migrations");
            }

            foreach (var migration in response.DocumentsWithMetaData)
            {
                var nonParsedId = migration.Id;
                var migrationId = long.Parse(nonParsedId, CultureInfo.InvariantCulture);
                AppliedVersionsInfo.AddAppliedMigration(migrationId);
            }
        }

        public void DeleteVersion(long version)
        {
            var response = _elasticClient.DeleteById(_indexName, _typeName, version.ToString(CultureInfo.InvariantCulture));
            if (!response.IsValid)
            {
                throw new InvalidOperationException();
            }
        }

        public void SaveVersionInfo(long version)
        {
            var response = _elasticClient.Index(new { }, _indexName, _typeName, version.ToString(CultureInfo.InvariantCulture));
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }
    }
}