using System.Globalization;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public class ElasticAppliedVersionsManager : IAppliedVersionsManager
    {
        private readonly IElasticApi _elasticApi;
        private readonly IElasticManagementApi _elasticManagementApi;

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
            var hits = _elasticApi.Scroll<MigrationDoc>(x => x.MatchAll());
            foreach (var hit in hits)
            {
                var migrationId = long.Parse(hit.Id, CultureInfo.InvariantCulture);
                AppliedVersionsInfo.AddAppliedMigration(migrationId);
            }         
        }

        public void DeleteVersion(long version)
        {
            _elasticApi.Delete<MigrationDoc>(version.ToString());
        }

        public void SaveVersionInfo(long version)
        {
            _elasticApi.Create(new MigrationDoc(), version.ToString());
        }
    }
}