using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public class ElasticAppliedVersionsManager : IAppliedVersionsManager
    {
        private readonly IElasticApi _elasticApi;
        private readonly IElasticManagementApi _elasticManagementApi;
        private IReadOnlyCollection<IHit<MigrationDoc>> _migrationDocs;

        public ElasticAppliedVersionsManager(IElasticApi elasticApi, IElasticManagementApi elasticManagementApi)
        {
            _elasticApi = elasticApi;
            _elasticManagementApi = elasticManagementApi;
        }

        public AppliedVersionsInfo AppliedVersionsInfo { get; private set; }
        public void LoadVersionInfo()
        {
            AppliedVersionsInfo = new AppliedVersionsInfo();

            var exists = _elasticManagementApi.TypeExists<MigrationDoc>();
            if (!exists)
            {
                return;
            }

            _elasticApi.Refresh<MigrationDoc>();
            _migrationDocs = _elasticApi.Scroll<MigrationDoc>(x => x.MatchAll().Version()).ToList();
            foreach (var migrationDoc in _migrationDocs)
            {
                var migrationId = long.Parse(migrationDoc.Id, CultureInfo.InvariantCulture);
                AppliedVersionsInfo.AddAppliedMigration(migrationId);
            }         
        }

        public void DeleteVersion(long version)
        {
            var migrationDoc = _migrationDocs.Single(x => string.Equals(x.Id, version.ToString(), StringComparison.OrdinalIgnoreCase));
            _elasticApi.Delete<MigrationDoc>(migrationDoc.Id, long.Parse(migrationDoc.Version));
        }

        public void SaveVersionInfo(long version)
        {
            _elasticApi.Create(new MigrationDoc(), version.ToString());
        }
    }
}