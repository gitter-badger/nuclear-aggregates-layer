using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;
using DoubleGis.Erm.Qds.Migrations.Extensions;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(15468, "Первичная миграция данных", "f.zaharov")]
    public sealed class Migration15468 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<OrderGridDoc15468>("Data.15468", "OrderGridDoc");
            context.ManagementApi.CreateIndex<OrderGridDoc15468>(GetDataIndexDescriptor());
            context.ManagementApi.AddAlias<OrderGridDoc15468>("Data");

            PutOrderGridDocMapping(context);
        }

        private static void PutOrderGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.ManagementApi.Map<OrderGridDoc15468>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.NotAnalyzed))
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
                    .Date(d => d.Name(n => n.BeginDistributionDate))
                    .Date(d => d.Name(n => n.EndDistributionDatePlan))
                    .Date(d => d.Name(n => n.EndDistributionDateFact))
                    .Date(d => d.Name(n => n.CreatedOn))
                    .Date(d => d.Name(n => n.ModifiedOn))
                    .Number(num => num.Name(n => n.HasDocumentsDebt).Type(NumberType.Byte))
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))
                    .Number(s => s.Name(n => n.PayablePlan).Type(NumberType.Double))
                    .MultiField(mf => mf
                        .Name(n => n.WorkflowStep)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.WorkflowStep)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.WorkflowStep.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        ))
                    .String(s => s.Name(n => n.WorkflowStepId).Index(FieldIndexOption.NotAnalyzed))
                    .Number(s => s.Name(n => n.AmountToWithdraw).Type(NumberType.Double))
                    .Number(s => s.Name(n => n.AmountWithdrawn).Type(NumberType.Double))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.Strict)

                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.NotAnalyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("OrderGridDoc");
        }

        private static Func<CreateIndexDescriptor, CreateIndexDescriptor> GetDataIndexDescriptor()
        {
            return x => x
            .NumberOfShards(1)
            .NumberOfReplicas(2)
            .Settings(s => s.Add("refresh_interval", "1s"))
            .Analysis(
                z => z.TokenFilters(p => p
                            .Add("ru_icu_collation", new IcuTokenFilter { Language = "ru" })
                            .Add("whitespace_regex", new PatternReplaceTokenFilter
                            {
                                Pattern = @"[!@#$%^&*()_+=\[{\]};:<>|./?,\\'""\-]*",
                                Replacement = " ",
                            })
                        )
                        .Analyzers(p => p
                            .Add("ru_searching", new StandardAnalyzer())
                            .Add("ru_sorting", new CustomAnalyzer
                            {
                                Tokenizer = "keyword",
                                Filter = new[] { "whitespace_regex", "trim", "ru_icu_collation" },
                            })
                        )
                );
        }

        private sealed class OrderGridDoc15468
        {
            public string Id { get; set; }
            public string Number { get; set; }

            public DateTime BeginDistributionDate { get; set; }
            public DateTime EndDistributionDatePlan { get; set; }
            public DateTime EndDistributionDateFact { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public byte HasDocumentsDebt { get; set; }

            public DateTime CreatedOn { get; set; }
            public DateTime? ModifiedOn { get; set; }

            public decimal PayablePlan { get; set; }
            public string WorkflowStep { get; set; }
            public int WorkflowStepId { get; set; }

            public decimal AmountToWithdraw { get; set; }
            public decimal AmountWithdrawn { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }
    }
}