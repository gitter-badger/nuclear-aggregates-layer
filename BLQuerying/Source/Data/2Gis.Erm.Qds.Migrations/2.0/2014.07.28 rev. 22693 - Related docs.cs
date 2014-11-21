using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(22693, "UserGridDoc, DepartmentGridDoc, OrderGridDoc, FirmGridDoc mapping", "m.pashuk")]
    public sealed class Migration22693 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutDepartmentGridDocMapping(context);
            PutUserGridDocMapping(context);
            PutCurrencyGridDocMapping(context);
            PutCountryGridDocMapping(context);
            PutOrgUnitGridDocMapping(context);
            PutLegalPersonGridDocMapping(context);
            PutBargainGridDocMapping(context);
            PutFirmGridDocMapping(context);
            PutOrderGridDocMapping(context);
        }

        private static void PutBargainGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<BargainGridDoc22693>("Data", "BargainGridDoc");
            context.ManagementApi.Map<BargainGridDoc22693>(m => m
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
                    .Number(s => s.Name(n => n.LegalPersonId).Type(NumberType.Long))
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
                    .Number(s => s.Name(n => n.OwnerCode).Type(NumberType.Long))
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

        private static void PutLegalPersonGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<LegalPersonGridDoc22693>("Data", "LegalPersonGridDoc");
            context.ManagementApi.Map<LegalPersonGridDoc22693>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))
                    .MultiField(mf => mf
                        .Name(n => n.LegalName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LegalName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LegalName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.ShortName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.ShortName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.ShortName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.Inn)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Inn)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Inn.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.Kpp)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Kpp)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Kpp.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.LegalAddress)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LegalAddress)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LegalAddress.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.PassportNumber)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.PassportNumber)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.PassportNumber.Suffix("sort"))
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

            context.ReplicationQueue.Add("LegalPersonGridDoc");
        }

        private static void PutOrgUnitGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<OrgUnitGridDoc22693>("Data", "OrgUnitGridDoc");
            context.ManagementApi.Map<OrgUnitGridDoc22693>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))
                    .Number(s => s.Name(n => n.DgppId).Type(NumberType.Integer))
                    .String(s => s.Name(n => n.ReplicationCode).Index(FieldIndexOption.No))
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
                    .String(s => s.Name(x => x.CountryId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.CountryName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.CountryName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.CountryName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Date(d => d.Name(n => n.FirstEmitDate))
                    .Date(d => d.Name(n => n.ErmLaunchDate))
                    .Date(d => d.Name(n => n.InfoRussiaLaunchDate))

                    .Boolean(b => b.Name(n => n.ErmLaunched))
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

            context.ReplicationQueue.Add("OrgUnitGridDoc");            
        }

        private static void PutCountryGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<CountryGridDoc22693>("Data", "CountryGridDoc");
            context.ManagementApi.Map<CountryGridDoc22693>(m => m
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
                    .String(s => s.Name(x => x.CurrencyId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.CurrencyName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.CurrencyName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.CurrencyName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Number(s => s.Name(n => n.IsoCode).Type(NumberType.Long))
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

            context.ReplicationQueue.Add("CountryGridDoc");
        }

        private static void PutCurrencyGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<CurrencyGridDoc22693>("Data", "CurrencyGridDoc");
            context.ManagementApi.Map<CurrencyGridDoc22693>(m => m
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
                    .Number(s => s.Name(n => n.IsoCode).Type(NumberType.Long))
                    .MultiField(mf => mf
                        .Name(n => n.Symbol)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Symbol)
                                .Index(FieldIndexOption.NotAnalyzed)
                            )
                            .String(s => s
                                .Name(n => n.Symbol.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .Boolean(b => b.Name(n => n.IsBase))
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

            context.ReplicationQueue.Add("CurrencyGridDoc");
        }

        private static void PutFirmGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<FirmGridDoc22693>("Data", "FirmGridDoc");

            context.ManagementApi.Map<FirmGridDoc22693>(m => m

                .Properties(p => p

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
                )
            );

            context.ReplicationQueue.Add("FirmGridDoc");
        }

        private static void PutOrderGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<OrderGridDoc22693>("Data", "OrderGridDoc");

            context.ManagementApi.Map<OrderGridDoc22693>(m => m

                .Properties(p => p

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
                    .String(s => s.Name(x => x.SourceOrganizationUnitId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.SourceOrganizationUnitName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.SourceOrganizationUnitName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.SourceOrganizationUnitName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.DestOrganizationUnitId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.DestOrganizationUnitName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.DestOrganizationUnitName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.DestOrganizationUnitName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.LegalPersonId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.LegalPersonName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LegalPersonName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LegalPersonName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.BargainId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.BargainNumber)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.BargainNumber)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.BargainNumber.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                )
            );

            context.ReplicationQueue.Add("OrderGridDoc");
        }

        private static void PutDepartmentGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<DepartmentGridDoc22693>("Data", "DepartmentGridDoc");
            context.ManagementApi.Map<DepartmentGridDoc22693>(m => m
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
                    .String(s => s.Name(x => x.ParentId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.ParentName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.ParentName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.ParentName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )

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

            context.ReplicationQueue.Add("DepartmentGridDoc");
        }

        private static void PutUserGridDocMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<UserGridDoc22693>("Data", "UserGridDoc");

            context.ManagementApi.Map<UserGridDoc22693>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .Number(s => s.Name(n => n.Id).Type(NumberType.Long))
                    .MultiField(mf => mf
                        .Name(n => n.Account)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.Account)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.Account.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.FirstName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.FirstName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.FirstName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.LastName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.LastName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.LastName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .MultiField(mf => mf
                        .Name(n => n.DisplayName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.DisplayName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.DisplayName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.ParentId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.ParentName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.ParentName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.ParentName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )
                    .String(s => s.Name(x => x.DepartmentId).Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(n => n.DepartmentName)
                        .Fields(f => f
                            .String(s => s
                                .Name(n => n.DepartmentName)
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_searching")
                                .SearchAnalyzer("ru_searching")
                            )
                            .String(s => s
                                .Name(n => n.DepartmentName.Suffix("sort"))
                                .Index(FieldIndexOption.Analyzed)
                                .IndexAnalyzer("ru_sorting"))
                        )
                    )

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

            context.ReplicationQueue.Add("UserGridDoc");
        }

        public sealed class FirmGridDoc22693
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
            //public string ClientId { get; set; }
            //public string ClientName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }
            //public string TerritoryId { get; set; }
            //public string TerritoryName { get; set; }
            public string OrganizationUnitId { get; set; }
            public string OrganizationUnitName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class OrderGridDoc22693
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

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class OrgUnitGridDoc22693
        {
            public long? Id { get; set; }
            public int? DgppId { get; set; }
            public string ReplicationCode { get; set; }
            public string Name { get; set; }
            public DateTime? FirstEmitDate { get; set; }
            public DateTime? ErmLaunchDate { get; set; }
            public DateTime? InfoRussiaLaunchDate { get; set; }
            public bool? ErmLaunched { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            // relations
            public string CountryId { get; set; }
            public string CountryName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class CurrencyGridDoc22693
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public long? IsoCode { get; set; }
            public string Symbol { get; set; }
            public bool? IsBase { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class CountryGridDoc22693
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public long? IsoCode { get; set; }
            public bool? IsDeleted { get; set; }

            // relations
            public string CurrencyId { get; set; }
            public string CurrencyName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class DepartmentGridDoc22693
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            // relations
            public string ParentId { get; set; }
            public string ParentName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class UserGridDoc22693
        {
            public long? Id { get; set; }
            public string Account { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

            //public ICollection<string> TerritoryIds { get; set; }
            //public ICollection<string> OrganizationUnitIds { get; set; }

            // relations
            public string ParentId { get; set; }
            public string ParentName { get; set; }
            public string DepartmentId { get; set; }
            public string DepartmentName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class LegalPersonGridDoc22693
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
            //public string ClientId { get; set; }
            //public string ClientName { get; set; }
            public string OwnerCode { get; set; }
            public string OwnerName { get; set; }

            public DocumentAuthorization Authorization { get; set; }
        }

        public sealed class BargainGridDoc22693
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