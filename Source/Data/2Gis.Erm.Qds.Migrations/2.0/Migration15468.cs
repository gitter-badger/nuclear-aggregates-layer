using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Common.Extensions;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;
using Nest.Resolvers;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(15468, "Миграция TerritoryDoc и RecordIdState", "f.zaharov")]
    public sealed class Migration15468 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            CreateTerritoryDocIndex(context);
            IndexTerritoryDocIndex(context);

            CreateRecordIdStateIndex(context);
            IndexRecordIdStateIndex(context);
        }

        private void IndexTerritoryDocIndex(IElasticSearchMigrationContext context)
        {
            context.RawDocumentIndexer.IndexAllDocuments("territorydoc");
        }

        private void IndexRecordIdStateIndex(IElasticSearchMigrationContext context)
        {
            context.RawDocumentIndexer.IndexAllDocuments("recordidstate");
        }

        private void CreateRecordIdStateIndex(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("metadata");

            var mapping = new RootObjectMapping
            {
                Dynamic = DynamicMappingOption.strict,
                DateDetection = false,
                NumericDetection = false,
                AllFieldMapping = new AllFieldMapping().SetDisabled(),
                IdFieldMapping = new IdFieldMapping().SetStored(true).SetPath("Id"),
                TypeNameMarker = "RecordIdState".MakePlural().ToLowerInvariant(),
                Properties = new Dictionary<string, IElasticType>
                {
                    { "Id".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                    { "RecordId".ToCamelCase(), new StringMapping { Index = FieldIndexOption.no } },
                },
            };

            var response = context.ElasticClient.Map(mapping, indexName, null, false);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }

        private void CreateTerritoryDocIndex(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("metadata");

            var mapping = new RootObjectMapping
            {
                Dynamic = DynamicMappingOption.strict,
                DateDetection = false,
                NumericDetection = false,
                AllFieldMapping = new AllFieldMapping().SetDisabled(),
                TypeNameMarker = "TerritoryDoc".MakePlural().ToLowerInvariant(),
                IdFieldMapping = new IdFieldMapping().SetStored(true).SetPath("Id"),
                Properties = new Dictionary<string, IElasticType>
                {
                    { "Name".ToCamelCase(), new StringMapping { Index = FieldIndexOption.no } },
                    { "Id".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                },
            };

            var response = context.ElasticClient.Map(mapping, indexName, null, false);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException(response.ConnectionStatus.Error.ExceptionMessage, response.ConnectionStatus.Error.OriginalException);
            }
        }
    }
}