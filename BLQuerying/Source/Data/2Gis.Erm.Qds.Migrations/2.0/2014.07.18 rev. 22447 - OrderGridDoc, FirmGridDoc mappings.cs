using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(22447, "OrderGridDoc, FirmGridDoc mappings", "m.pashuk")]
    public sealed class Migration22447 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutFirmGridDocMapping(context);
            PutOrderGridDocMapping(context);
        }

        private static void PutFirmGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<FirmGridDoc22447>("Data", "FirmGridDoc");

            context.ManagementApi.Map<FirmGridDoc22447>(m => m
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
                    //.String(s => s.Name(n => n.ClientId).Index(FieldIndexOption.NotAnalyzed))
                    //.MultiField(mf => mf
                    //    .Name(n => n.ClientName)
                    //    .Fields(f => f
                    //        .String(s => s
                    //            .Name(n => n.ClientName)
                    //            .Index(FieldIndexOption.Analyzed)
                    //            .IndexAnalyzer("ru_searching")
                    //            .SearchAnalyzer("ru_searching")
                    //        )
                    //        .String(s => s
                    //            .Name(n => n.ClientName.Suffix("sort"))
                    //            .Index(FieldIndexOption.Analyzed)
                    //            .IndexAnalyzer("ru_sorting"))
                    //    )
                    //)
                    .Number(num => num.Name(n => n.PromisingScore).Type(NumberType.Integer))
                    .Date(d => d.Name(n => n.LastQualifyTime))
                    .Date(d => d.Name(n => n.LastDisqualifyTime))
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))
                    .Boolean(b => b.Name(n => n.ClosedForAscertainment))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.Strict)
                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.NotAnalyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("FirmGridDoc");
        }

        private static void PutOrderGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<OrderGridDoc22447>("Data", "OrderGridDoc");

            context.ManagementApi.Map<OrderGridDoc22447>(m => m

                .Properties(p => p

                    .String(s => s.Name(x => x.FirmId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.FirmName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.FirmName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.FirmName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("OrderGridDoc");
        }

        private sealed class OrderGridDoc22447 :  IAuthorizationDoc
        {
            public long Id { get; set; }
            public string Number { get; set; }

            public DateTime BeginDistributionDate { get; set; }
            public DateTime EndDistributionDatePlan { get; set; }
            public DateTime EndDistributionDateFact { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public byte HasDocumentsDebt { get; set; }

            public DateTime CreatedOn { get; set; }
            public DateTime? ModifiedOn { get; set; }

            public double PayablePlan { get; set; }
            public string WorkflowStep { get; set; }

            public double AmountToWithdraw { get; set; }
            public double AmountWithdrawn { get; set; }

            // relations
            public string FirmId { get; set; }
            public string FirmName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class FirmGridDoc22447 : IAuthorizationDoc
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int PromisingScore { get; set; }
            public DateTime? LastQualifyTime { get; set; }
            public DateTime? LastDisqualifyTime { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool ClosedForAscertainment { get; set; }

            // relations
            //public string ClientId { get; set; }
            //public string ClientName { get; set; }
            //public string OwnerCode { get; set; }
            //public string OwnerName { get; set; }
            //public string TerritoryId { get; set; }
            //public string TerritoryName { get; set; }
            //public string OrganizationUnitId { get; set; }
            //public string OrganizationUnitName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }
    }
}