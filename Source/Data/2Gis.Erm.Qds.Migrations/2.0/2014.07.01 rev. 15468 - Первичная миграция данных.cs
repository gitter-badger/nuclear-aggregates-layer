using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Docs;
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
            context.NestSettings.RegisterType<OrderGridDoc15468>("Data.15468", "OrderGridDoc");
            context.ElasticManagementApi.CreateIndex<OrderGridDoc15468>(GetDataIndexDescriptor());
            context.ElasticManagementApi.AddAlias<OrderGridDoc15468>("Data");

            PutOrderDocMapping(context);

            /*
             * Cut for ERM-4267
                PutTerritoryDocMapping(context);
                PutUserDocMapping(context);
                PutClientGridDocMapping(context);
                PutFirmGridDocMapping(context);
             */
        }


        private static void PutFirmGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<FirmGridDoc15468>("Data.15468", "FirmGridDoc");
            context.ElasticManagementApi.CreateIndex<FirmGridDoc15468>(GetDataIndexDescriptor());
            context.ElasticManagementApi.AddAlias<FirmGridDoc15468>("Data");

            context.ElasticManagementApi.Map<FirmGridDoc15468>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.Name)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Name)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Name.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.ClientId).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.ClientName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.ClientName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.ClientName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.OwnerCode).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.OwnerName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OwnerName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OwnerName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.TerritoryId).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.TerritoryName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.TerritoryName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.TerritoryName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Number(num => num.Name(n => n.PromisingScore).Type(NumberType.integer))
                    .Date(d => d.Name(n => n.LastQualifyTime))
                    .Date(d => d.Name(n => n.LastDisqualifyTime))
                    .String(s => s.Name(n => n.OrganizationUnitId).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.OrganizationUnitName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OrganizationUnitName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OrganizationUnitName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))
                    .Boolean(b => b.Name(n => n.ClosedForAscertainment))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.strict)
                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.not_analyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("FirmGridDoc");
        }

        private static void PutClientGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.ElasticManagementApi.Map<ClientGridDoc15468>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                    .NestedObject<LegalPersonDoc15468>(no => no
                            .Name(n => n.LegalPersons.First())
                            .Properties(pp => pp
                                .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                .Boolean(b => b.Name(n => n.IsActive))
                                .Boolean(b => b.Name(n => n.IsDeleted))
                                .NestedObject<AccountDoc15468>(noo => noo
                                    .Name(n => n.Accounts.First())
                                    .Properties(ppp => ppp
                                        .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                        .Number(s => s.Name(n => n.Balance).Type(NumberType.@double))
                                        .Boolean(b => b.Name(n => n.IsActive))
                                        .Boolean(b => b.Name(n => n.IsDeleted))
                                    )
                                )
                            )
                        )
                    .MultiField(mf => mf
                        .Name(n => n.Name)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Name)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Name.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.MainAddress)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainAddress)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainAddress.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.TerritoryId).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.TerritoryName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.TerritoryName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.TerritoryName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(n => n.OwnerCode).Index(FieldIndexOption.not_analyzed)
                    )
                    .MultiField(mf => mf
                        .Name(n => n.OwnerName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.OwnerName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.OwnerName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Boolean(b => b.Name(n => n.IsAdvertisingAgency))
                    .Boolean(b => b.Name(n => n.IsActive))
                    .Boolean(b => b.Name(n => n.IsDeleted))
                    //.Boolean(b => b.Name(n => n.HasAccountDebt))
                    .Date(d => d.Name(n => n.CreatedOn))
                    .Date(d => d.Name(n => n.LastQualifyTime))
                    .Date(d => d.Name(n => n.LastDisqualifyTime))
                    .String(s => s.Name(n => n.MainFirmId).Index(FieldIndexOption.not_analyzed))
                    .MultiField(mf => mf
                        .Name(n => n.MainFirmName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainFirmName)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainFirmName.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.MainPhoneNumber)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.MainPhoneNumber)
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.MainPhoneNumber.Suffix("sort"))
                                .Index(FieldIndexOption.analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Number(num => num.Name(n => n.InformationSourceEnum).Type(NumberType.@short))
                    .Object<DocumentAuthorization>(o => o
                        .Dynamic(DynamicMappingOption.strict)

                        .Name(n => n.Authorization)
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.not_analyzed))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("ClientGridDoc");
        }

        private static void PutUserDocMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<UserDoc15468>("Metadata.15468", "UserDoc");
            //context.ElasticManagementApi.CreateIndex<UserDoc15468>(GetMetadataIndexDescriptor(), "Metadata");

            context.ElasticManagementApi.Map<UserDoc15468>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                    .String(s => s.Name(n => n.Name).Index(FieldIndexOption.no))

                    .Object<UserDoc15468.OperationPermission15468>(o => o
                        .Name(n => n.Permissions.First())
                        .Dynamic(DynamicMappingOption.strict)

                        .Properties(pp => pp
                            .String(s => s.Name(n => n.Operation).Index(FieldIndexOption.no))
                            .String(s => s.Name(n => n.Tags).Index(FieldIndexOption.no))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("UserDoc");
        }

        void PutOrderDocMapping(IElasticSearchMigrationContext context)
        {
            context.ElasticManagementApi.Map<OrderGridDoc15468>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
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
                    .String(s => s.Name(n => n.WorkflowStepId).Index(FieldIndexOption.not_analyzed))
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

        private void PutTerritoryDocMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<TerritoryDoc15468>("Metadata", "TerritoryDoc");

            context.ElasticManagementApi.Map<TerritoryDoc15468>(m => m
                .Dynamic(DynamicMappingOption.strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                    .String(s => s.Name(n => n.Name).Index(FieldIndexOption.no))
                )
            );

            context.ReplicationQueue.Add("TerritoryDoc");
        }

        private sealed class UserDoc15468
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public IEnumerable<OperationPermission15468> Permissions { get; set; }

            public sealed class OperationPermission15468
            {
                public string Operation { get; set; }
                public IEnumerable<string> Tags { get; set; }
            }
        }

        private sealed class ClientGridDoc15468
        {
            public LegalPersonDoc15468[] LegalPersons { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string MainAddress { get; set; }
            public string TerritoryId { get; set; }
            public string TerritoryName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            public bool IsAdvertisingAgency { get; set; }

            public string MainFirmId { get; set; }
            public string MainFirmName { get; set; }
            public string MainPhoneNumber { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime LastQualifyTime { get; set; }
            public DateTime? LastDisqualifyTime { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public int InformationSourceEnum { get; set; }
            //public bool HasAccountDebt { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public class LegalPersonDoc15468
        {
            public string Id { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public AccountDoc15468[] Accounts { get; set; }
        }

        public class AccountDoc15468
        {
            public string Id { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public double Balance { get; set; }
        }

        private sealed class FirmGridDoc15468
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string ClientId { get; set; }
            public string ClientName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            public string TerritoryId { get; set; }
            public string TerritoryName { get; set; }
            public int PromisingScore { get; set; }
            public DateTime? LastQualifyTime { get; set; }
            public DateTime? LastDisqualifyTime { get; set; }
            public string OrganizationUnitId { get; set; }
            public string OrganizationUnitName { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool ClosedForAscertainment { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public class OrderGridDoc15468
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

        private sealed class TerritoryDoc15468
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}