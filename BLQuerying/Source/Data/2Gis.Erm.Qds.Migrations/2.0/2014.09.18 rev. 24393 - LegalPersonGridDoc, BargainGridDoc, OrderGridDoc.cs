using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(24393, "LegalPersonGridDoc, BargainGridDoc, OrderGridDoc", "m.pashuk")]
    public sealed class Migration24393 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutOrderGridDocMapping(context);
            PutBargainGridDocMapping(context);
            PutLegalPersonGridDocMapping(context);
        }

        private static void PutOrderGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<OrderGridDoc24393>("Data", "OrderGridDoc");
            context.ManagementApi.Map<OrderGridDoc24393>(m => m

                .Properties(p => p

                    .String(s => s.Name(n => n.AccountId).Index(FieldIndexOption.NotAnalyzed))
                    .String(s => s.Name(n => n.ClientId).Index(FieldIndexOption.NotAnalyzed))
                    .String(s => s.Name(n => n.DealId).Index(FieldIndexOption.NotAnalyzed))
                )
            );

            context.ReplicationQueue.Add("OrderGridDoc");
        }

        private static void PutLegalPersonGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<LegalPersonGridDoc24393>("Data", "LegalPersonGridDoc");
            context.ManagementApi.Map<LegalPersonGridDoc24393>(m => m

                .Properties(p => p

                    .String(s => s.Name(n => n.ClientId).Index(FieldIndexOption.NotAnalyzed))
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

            context.ReplicationQueue.Add("LegalPersonGridDoc");
        }

        private static void PutBargainGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<BargainGridDoc24393>("Data", "BargainGridDoc");
            context.ManagementApi.Map<BargainGridDoc24393>(m => m

                .Properties(p => p

                    .String(s => s.Name(n => n.ClientId).Index(FieldIndexOption.NotAnalyzed))
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

            context.ReplicationQueue.Add("BargainGridDoc");
        }

        public sealed class OrderGridDoc24393
        {
            public long? Id { get; set; }
            public string Number { get; set; }

            public DateTime? BeginDistributionDate { get; set; }
            public DateTime? EndDistributionDatePlan { get; set; }
            public DateTime? EndDistributionDateFact { get; set; }

            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }
            public byte? HasDocumentsDebt { get; set; }

            public DateTime? CreatedOn { get; set; }
            public DateTime? ModifiedOn { get; set; }

            public double? PayablePlan { get; set; }
            public string WorkflowStep { get; set; }

            public double? AmountToWithdraw { get; set; }
            public double? AmountWithdrawn { get; set; }

            // relations
            public string FirmId { get; set; }
            public string FirmName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            public string SourceOrganizationUnitId { get; set; }
            public string SourceOrganizationUnitName { get; set; }
            public string DestOrganizationUnitId { get; set; }
            public string DestOrganizationUnitName { get; set; }
            public string LegalPersonId { get; set; }
            public string LegalPersonName { get; set; }
            public string BargainId { get; set; }
            public string BargainNumber { get; set; }
            public string AccountId { get; set; }
            public string ClientId { get; set; }
            public string DealId { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class LegalPersonGridDoc24393
        {
            public long? Id { get; set; }
            public string LegalName { get; set; }
            public string ShortName { get; set; }
            public string Inn { get; set; }
            public string Kpp { get; set; }
            public string LegalAddress { get; set; }
            public string PassportNumber { get; set; }
            public DateTime? CreatedOn { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            // relations
            public string ClientId { get; set; }
            public string ClientName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        private sealed class BargainGridDoc24393
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
            public string ClientId { get; set; }
            public string ClientName { get; set; }
            //public string BranchOfficeId { get; set; }
            //public string BranchOfficeName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }
    }
}