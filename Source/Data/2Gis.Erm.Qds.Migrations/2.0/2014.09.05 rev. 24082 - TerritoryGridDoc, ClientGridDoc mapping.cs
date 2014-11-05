using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(24082, "TerritoryGridDoc, ClientGridDoc mapping", "m.pashuk")]
    public sealed class Migration24082 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            // фикс бага в миграции 22693
            PutBargainGridDocMapping(context);

            PutClientGridDocMapping(context);
            PutTerritoryGridDocMapping(context);
            PutFirmGridDocMapping(context);
        }

        private static void PutClientGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<ClientGridDoc24082>("Data", "ClientGridDoc");

            context.ManagementApi.Map<ClientGridDoc24082>(m => m

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))

                    .MultiField(mf => mf
                        .Name(n => n.Name)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Name)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Name.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.MainAddress)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainAddress)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainAddress.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.MainFirmId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.MainFirmName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainFirmName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainFirmName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.MainPhoneNumber)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainPhoneNumber)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainPhoneNumber.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.TerritoryId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.TerritoryName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.TerritoryName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.TerritoryName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.OwnerCode).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.OwnerName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OwnerName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OwnerName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )

                    .Number(s => s.Name(n => n.InformationSourceEnum).Type(NumberType.Integer))

                    .Date(d => d.Name(n => n.CreatedOn))
                    .Date(d => d.Name(n => n.LastQualifyTime))
                    .Date(d => d.Name(n => n.LastDisqualifyTime))

                    .Boolean(b => b.Name(n => n.IsAdvertisingAgency))
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))

                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.Strict)
                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.NotAnalyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("ClientGridDoc");
        }

        private static void PutBargainGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<BargainGridDoc24082>("Data", "BargainGridDoc");
            context.ManagementApi.DeleteMapping<BargainGridDoc24082>();
            context.ManagementApi.Map<BargainGridDoc24082>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))
                    .MultiField(mf => mf
                        .Name(n => n.Number)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Number)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Number.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.LegalPersonId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.LegalPersonLegalName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LegalPersonLegalName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LegalPersonLegalName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.LegalPersonLegalAddress)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LegalPersonLegalAddress)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LegalPersonLegalAddress.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.OwnerCode).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.OwnerName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OwnerName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OwnerName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Number(s => s.Name(n => n.BargainKindEnum).Type(NumberType.Integer))
                    .MultiField(mf => mf
                        .Name(n => n.BargainKind)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.BargainKind)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.BargainKind.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )


                    .Date(d => d.Name(n => n.BargainEndDate))
                    .Date(d => d.Name(n => n.CreatedOn))

                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))

                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.Strict)
                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.NotAnalyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("BargainGridDoc");
        }

        private static void PutFirmGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<FirmGridDoc24082>("Data", "FirmGridDoc");

            context.ManagementApi.Map<FirmGridDoc24082>(m => m

                .Properties(p => p

                    .String(s => s.Name(x => x.TerritoryId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.TerritoryName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.TerritoryName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.TerritoryName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.ClientId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.ClientName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.ClientName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.ClientName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("FirmGridDoc");
        }

        private static void PutTerritoryGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<TerritoryGridDoc24082>("Data", "TerritoryGridDoc");

            context.ManagementApi.Map<TerritoryGridDoc24082>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))
                    .MultiField(mf => mf
                        .Name(n => n.Name)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Name)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Name.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.OrganizationUnitId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.OrganizationUnitName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OrganizationUnitName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OrganizationUnitName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.Strict)
                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.NotAnalyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("TerritoryGridDoc");
        }

        private sealed class ClientGridDoc24082
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public string MainAddress { get; set; }
            public string TerritoryId { get; set; }
            public string TerritoryName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            public bool? IsAdvertisingAgency { get; set; }

            public string MainFirmId { get; set; }
            public string MainFirmName { get; set; }
            public string MainPhoneNumber { get; set; }
            public DateTime? CreatedOn { get; set; }
            public DateTime? LastQualifyTime { get; set; }
            public DateTime? LastDisqualifyTime { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public InformationSource InformationSourceEnum { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        private sealed class FirmGridDoc24082
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public int? PromisingScore { get; set; }
            public DateTime? LastQualifyTime { get; set; }
            public DateTime? LastDisqualifyTime { get; set; }

            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }
            public bool? ClosedForAscertainment { get; set; }

            // relations
            public string ClientId { get; set; }
            public string ClientName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            public string TerritoryId { get; set; }
            public string TerritoryName { get; set; }
            public string OrganizationUnitId { get; set; }
            public string OrganizationUnitName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        private sealed class TerritoryGridDoc24082
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public string OrganizationUnitId { get; set; }
            public string OrganizationUnitName { get; set; }
            public bool? IsActive { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        private sealed class BargainGridDoc24082
        {
            public long? Id { get; set; }
            public string Number { get; set; }
            public DateTime? BargainEndDate { get; set; }
            public BargainKind BargainKindEnum { get; set; }
            public string BargainKind { get; set; }
            public DateTime? CreatedOn { get; set; }

            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            // relations
            public string LegalPersonId { get; set; }
            public string LegalPersonLegalName { get; set; }
            public string LegalPersonLegalAddress { get; set; }
            //public string ClientId { get; set; }
            //public string ClientName { get; set; }
            //public string BranchOfficeId { get; set; }
            //public string BranchOfficeName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }
    }
}