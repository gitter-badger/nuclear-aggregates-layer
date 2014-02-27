using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Common.Extensions;
using DoubleGis.Erm.Qds.Migrations.Base;
using DoubleGis.Erm.Qds.Migrations.Extensions;

using Nest;
using Nest.Resolvers;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(13546, "Создание поисковой инфраструктуры", "m.pashuk")]
    public sealed class Migration13546 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            TryCreateMetadataIndex(context);
            PutMigrationsMapping(context);
            PutUserDocMapping(context);
            IndexAllUserDoc(context);

            TryCreateDataIndex(context);
            PutClientGridDocMapping(context);
            IndexAllClientGridDoc(context);
        }

        private static void IndexAllClientGridDoc(IElasticSearchMigrationContext context)
        {
            context.RawDocumentIndexer.IndexAllDocuments("clientgriddoc");
        }

        private static void IndexAllUserDoc(IElasticSearchMigrationContext context)
        {
            context.RawDocumentIndexer.IndexAllDocuments("userdoc");
        }

        private static void PutClientGridDocMapping(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("data");

            var mapping = new RootObjectMapping
            {
                Dynamic = DynamicMappingOption.strict,
                DateDetection = false,
                NumericDetection = false,
                AllFieldMapping = new AllFieldMapping().SetDisabled(),
                IdFieldMapping = new IdFieldMapping().SetStored(true).SetPath("Id"),
                TypeNameMarker = "ClientGridDoc".MakePlural().ToLowerInvariant(),
                Properties = new Dictionary<string, IElasticType>
                {
                    { "Id".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                    { "ReplicationCode".ToCamelCase(), new StringMapping { Index = FieldIndexOption.no } },
                    {
                        "Name".ToCamelCase(), new MultiFieldMapping
                        {
                            Fields = new Dictionary<string, IElasticCoreType>
                            {
                                {
                                    "Name".ToCamelCase(),
                                    new StringMapping
                                        {
                                            Index = FieldIndexOption.analyzed,
                                            IndexAnalyzer = "ru_searching",
                                            SearchAnalyzer = "ru_searching"
                                        }
                                },
                                {
                                    "Name".ToCamelCase() + ".sort",
                                    new StringMapping { Index = FieldIndexOption.analyzed, IndexAnalyzer = "ru_sorting" }
                                },
                            }
                        }
                    },
                    {
                        "MainAddress".ToCamelCase(), new MultiFieldMapping
                        {
                            Fields = new Dictionary<string, IElasticCoreType>
                            {
                                {
                                    "MainAddress".ToCamelCase() + ".sort",
                                    new StringMapping { Index = FieldIndexOption.analyzed, IndexAnalyzer = "ru_sorting" }
                                },
                            }
                        }
                    },
                    { "TerritoryId".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                    {
                        "TerritoryName".ToCamelCase(), new MultiFieldMapping
                        {
                            Fields = new Dictionary<string, IElasticCoreType>
                            {
                                {
                                    "TerritoryName".ToCamelCase() + ".sort",
                                    new StringMapping { Index = FieldIndexOption.analyzed, IndexAnalyzer = "ru_sorting" }
                                },
                            }
                        }
                    },
                    { "OwnerCode".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                    {
                        "OwnerName".ToCamelCase(), new MultiFieldMapping
                        {
                            Fields = new Dictionary<string, IElasticCoreType>
                            {
                                {
                                    "OwnerName".ToCamelCase() + ".sort",
                                    new StringMapping { Index = FieldIndexOption.analyzed, IndexAnalyzer = "ru_sorting" }
                                },
                            },
                        }
                    },
                    { "IsActive".ToCamelCase(), new BooleanMapping() },
                    { "IsDeleted".ToCamelCase(), new BooleanMapping() },
                    { "CreatedOn".ToCamelCase(), new DateMapping() },
                    {
                        "InformationSource".ToCamelCase(), new MultiFieldMapping
                        {
                            Fields = new Dictionary<string, IElasticCoreType>
                            {
                                {
                                    "InformationSource".ToCamelCase() + ".sort",
                                    new StringMapping { Index = FieldIndexOption.analyzed, IndexAnalyzer = "ru_sorting" }
                                },
                            }
                        }
                    },
                    {
                        "Auth".ToCamelCase(), new ObjectMapping
                        {
                            Dynamic = DynamicMappingOption.strict,
                            Properties = new Dictionary<string, IElasticType>
                            {
                                { "Tags".ToCamelCase(), new StringMapping { Index = FieldIndexOption.not_analyzed } }
                            },
                        }
                    },
                },
            };

            var response = context.ElasticClient.Map(mapping, indexName, null, false);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }

        private static void TryCreateDataIndex(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("data");

            var getResponse = context.ElasticClient.GetIndexSettings(indexName);
            if (getResponse.IsValid)
            {
                return;
            }

            var indexSettings = new IndexSettings
            {
                { "number_of_shards", 1 },
                { "number_of_replicas", 1 },
                { "refresh_interval", -1 },
            };

            var tokenFilters = indexSettings.Analysis.TokenFilters;
            tokenFilters.Add("ru_icu_collation", new IcuTokenFilter { Language = "ru" });
            tokenFilters.Add(
            "whitespace_regex",
            new PatternReplaceTokenFilter
            {
                Pattern = @"[!@#$%^&*()_+=\[{\]};:<>|./?,\\'""\-]*",
                Replacement = " ",
            });

            var charFilters = indexSettings.Analysis.CharFilters;
            charFilters.Add("ru_charfilter", new MappingCharFilter { Mappings = new[] { "Ё=>Е", "ё=>е" } });

            var analyzers = indexSettings.Analysis.Analyzers;
            analyzers.Add(
            "ru_sorting",
            new CustomAnalyzer
            {
                Tokenizer = "keyword",
                Filter = new[] { "whitespace_regex", "trim", "ru_icu_collation" },
            });
            analyzers.Add(
            "ru_searching",
            new CustomAnalyzer
            {
                Tokenizer = "keyword",
                Filter = new[] { "lowercase" },
                CharFilter = new[] { "ru_charfilter" },
            });

            var response = context.ElasticClient.CreateIndex(indexName, indexSettings);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }

        private static void PutUserDocMapping(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("metadata");

            var mapping = new RootObjectMapping
            {
                Dynamic = DynamicMappingOption.strict,
                DateDetection = false,
                NumericDetection = false,
                AllFieldMapping = new AllFieldMapping().SetDisabled(),

                TypeNameMarker = "UserDoc".MakePlural().ToLowerInvariant(),
                IdFieldMapping = new IdFieldMapping().SetStored(true).SetPath("Id"),
                Properties = new Dictionary<string, IElasticType>
                {
                    { "Name".ToCamelCase(), new StringMapping { Index = FieldIndexOption.no } },
                    { "Tags".ToCamelCase(), new StringMapping { Index = FieldIndexOption.not_analyzed } },
                    { "Id".ToCamelCase(), new NumberMapping { Type = NumberType.@long.ToString() } },
                    {
                        "Auth".ToCamelCase(), new ObjectMapping
                        {
                            Dynamic = DynamicMappingOption.strict,
                            Properties = new Dictionary<string, IElasticType>
                            {
                                { "Tags".ToCamelCase(), new StringMapping { Index = FieldIndexOption.not_analyzed } }
                            },
                        }
                    },
                },
            };

            var response = context.ElasticClient.Map(mapping, indexName, null, false);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }

        private static void PutMigrationsMapping(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("metadata");

            var mapping = new RootObjectMapping
            {
                Dynamic = DynamicMappingOption.strict,
                DateDetection = false,
                NumericDetection = false,
                AllFieldMapping = new AllFieldMapping().SetDisabled(),

                TypeNameMarker = "MigrationDoc".MakePlural().ToLowerInvariant(),
            };

            var response = context.ElasticClient.Map(mapping, indexName, null, false);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }

        private static void TryCreateMetadataIndex(IElasticSearchMigrationContext context)
        {
            var indexName = context.GetIndexName("metadata");

            var getResponse = context.ElasticClient.GetIndexSettings(indexName);
            if (getResponse.IsValid)
            {
                return;
            }

            var indexSettings = new IndexSettings
            {
                { "number_of_shards", 1 },
                { "number_of_replicas", 1 },
                { "refresh_interval", -1 },
            };

            var response = context.ElasticClient.CreateIndex(indexName, indexSettings);
            if (!response.ConnectionStatus.Success)
            {
                throw new InvalidOperationException();
            }
        }
    }
}