using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(21859, "OrderGridDoc, RecordIdState mappings", "m.pashuk")]
    public sealed class Migration21859 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutRecordIdStateMapping(context);
            PutOrderDocMapping(context);
        }

        private static void PutRecordIdStateMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<RecordIdState21859>("Metadata", "RecordIdState");

            context.ElasticManagementApi.DeleteMapping<RecordIdState21859>();
            context.ElasticManagementApi.Map<RecordIdState21859>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.RecordId).Index(FieldIndexOption.no))
                )
            );
        }

        private static void PutOrderDocMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<OrderGridDoc21859>("Data", "OrderGridDoc");

            context.ElasticManagementApi.DeleteMapping<OrderGridDoc21859>();
            context.ElasticManagementApi.Map<OrderGridDoc21859>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p

                    .Number(s => s.Name(n => n.Id).Type(NumberType.@long))

                    .MultiField(mf => mf
                        .Name(n => n.Number)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Number)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Number.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Date(d => d.Name(n => n.BeginDistributionDate))
                    .Date(d => d.Name(n => n.EndDistributionDatePlan))
                    .Date(d => d.Name(n => n.EndDistributionDateFact))
                    .Date(d => d.Name(n => n.CreatedOn))
                    .Date(d => d.Name(n => n.ModifiedOn))
                    .Number(num => num.Name(n => n.HasDocumentsDebt).Type(NumberType.@byte))
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))
                    .Number(s => s.Name(n => n.PayablePlan).Type(NumberType.@double))
                    .MultiField(mf => mf
                        .Name(n => n.WorkflowStep)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.WorkflowStep)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.WorkflowStep.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        ))
                    .Number(s => s.Name(n => n.AmountToWithdraw).Type(NumberType.@double))
                    .Number(s => s.Name(n => n.AmountWithdrawn).Type(NumberType.@double))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.strict)

                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.not_analyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("OrderGridDoc");
        }

        private sealed class RecordIdState21859
        {
            public string RecordId { get; set; }
        }

        private sealed class OrderGridDoc21859
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

            public DocumentAuthorization Authorization { get; set; }
        }
    }
}