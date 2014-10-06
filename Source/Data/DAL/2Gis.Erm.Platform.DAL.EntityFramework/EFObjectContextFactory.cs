using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Model.EntityFramework.Mapping;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFObjectContextFactory : IEFObjectContextFactory
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, DbCompiledModel> _dbModelCache = new Dictionary<string, DbCompiledModel>();

        private readonly IConnectionStringSettings _connectionStringSettings;

        public EFObjectContextFactory(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public ObjectContext CreateObjectContext(DomainContextMetadata domainContextMetadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(domainContextMetadata.ConnectionStringName);
            var sqlConnection = new SqlConnection(connectionString);

            var dbModel = GetDbModel(domainContextMetadata, sqlConnection);


            return dbModel.CreateObjectContext<ObjectContext>(sqlConnection);
        }

        private DbCompiledModel GetDbModel(DomainContextMetadata contextMetadata, SqlConnection sqlConnection)
        {
            DbCompiledModel dbModel;
            if (_dbModelCache.TryGetValue(contextMetadata.PathToEdmx, out dbModel))
            {
                return dbModel;
            }

            lock (_syncRoot)
            {
                if (_dbModelCache.TryGetValue(contextMetadata.PathToEdmx, out dbModel))
                {
                    return dbModel;
                }

                dbModel = GetDbModel(sqlConnection);
                _dbModelCache.Add(contextMetadata.PathToEdmx, dbModel);
            }

            return dbModel;
        }

        // FIXME {a.tukaev, 06.10.2014}: Зарефакторить / описать в метаданных ерм
        private DbCompiledModel GetDbModel(DbConnection connection)
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Configurations.Add(new AppointmentBaseMap());
            modelBuilder.Configurations.Add(new AppointmentReferenceMap());
            modelBuilder.Configurations.Add(new PhonecallBaseMap());
            modelBuilder.Configurations.Add(new PhonecallReferenceMap());
            modelBuilder.Configurations.Add(new TaskBaseMap());
            modelBuilder.Configurations.Add(new TaskReferenceMap());
            modelBuilder.Configurations.Add(new AccountDetailMap());
            modelBuilder.Configurations.Add(new AccountMap());
            modelBuilder.Configurations.Add(new AdsTemplatesAdsElementTemplateMap());
            modelBuilder.Configurations.Add(new AdvertisementElementDenialReasonMap());
            modelBuilder.Configurations.Add(new AdvertisementElementMap());
            modelBuilder.Configurations.Add(new AdvertisementElementStatusMap());
            modelBuilder.Configurations.Add(new AdvertisementElementTemplateMap());
            modelBuilder.Configurations.Add(new AdvertisementMap());
            modelBuilder.Configurations.Add(new AdvertisementTemplateMap());
            modelBuilder.Configurations.Add(new AfterSaleServiceActivityMap());
            modelBuilder.Configurations.Add(new AssociatedPositionMap());
            modelBuilder.Configurations.Add(new AssociatedPositionsGroupMap());
            modelBuilder.Configurations.Add(new BargainFileMap());
            modelBuilder.Configurations.Add(new BargainMap());
            modelBuilder.Configurations.Add(new BargainTypeMap());
            modelBuilder.Configurations.Add(new BillMap());
            modelBuilder.Configurations.Add(new BranchOfficeOrganizationUnitMap());
            modelBuilder.Configurations.Add(new BranchOfficeMap());
            modelBuilder.Configurations.Add(new ChargeMap());
            modelBuilder.Configurations.Add(new ChargesHistoryMap());
            modelBuilder.Configurations.Add(new ClientMap());
            modelBuilder.Configurations.Add(new ContactMap());
            modelBuilder.Configurations.Add(new ContributionTypeMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new CurrencyRateMap());
            modelBuilder.Configurations.Add(new DealMap());
            modelBuilder.Configurations.Add(new DenialReasonMap());
            modelBuilder.Configurations.Add(new DeniedPositionMap());
            modelBuilder.Configurations.Add(new LegalPersonProfileMap());
            modelBuilder.Configurations.Add(new LegalPersonMap());
            modelBuilder.Configurations.Add(new LimitMap());
            modelBuilder.Configurations.Add(new LockDetailMap());
            modelBuilder.Configurations.Add(new LockMap());
            modelBuilder.Configurations.Add(new OperationTypeMap());
            modelBuilder.Configurations.Add(new OrderFileMap());
            modelBuilder.Configurations.Add(new OrderPositionAdvertisementMap());
            modelBuilder.Configurations.Add(new OrderPositionMap());
            modelBuilder.Configurations.Add(new OrderProcessingRequestMessageMap());
            modelBuilder.Configurations.Add(new OrderProcessingRequestMap());
            modelBuilder.Configurations.Add(new OrderReleaseTotalMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrdersRegionalAdvertisingSharingMap());
            modelBuilder.Configurations.Add(new OrderValidationResultMap());
            modelBuilder.Configurations.Add(new OrderValidationRuleGroupDetailMap());
            modelBuilder.Configurations.Add(new OrderValidationRuleGroupMap());
            modelBuilder.Configurations.Add(new OrganizationUnitMap());
            modelBuilder.Configurations.Add(new PlatformMap());
            modelBuilder.Configurations.Add(new PositionCategoryMap());
            modelBuilder.Configurations.Add(new PositionChildrenMap());
            modelBuilder.Configurations.Add(new PositionMap());
            modelBuilder.Configurations.Add(new PricePositionMap());
            modelBuilder.Configurations.Add(new PriceMap());
            modelBuilder.Configurations.Add(new PrintFormTemplateMap());
            modelBuilder.Configurations.Add(new ProjectMap());
            modelBuilder.Configurations.Add(new RegionalAdvertisingSharingMap());
            modelBuilder.Configurations.Add(new ReleaseInfoMap());
            modelBuilder.Configurations.Add(new ReleasesWithdrawalMap());
            modelBuilder.Configurations.Add(new ReleasesWithdrawalsPositionMap());
            modelBuilder.Configurations.Add(new ReleaseValidationResultMap());
            modelBuilder.Configurations.Add(new ThemeCategoryMap());
            modelBuilder.Configurations.Add(new ThemeOrganizationUnitMap());
            modelBuilder.Configurations.Add(new ThemeMap());
            modelBuilder.Configurations.Add(new ThemeTemplateMap());
            modelBuilder.Configurations.Add(new WithdrawalInfoMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CategoryFirmAddressMap());
            modelBuilder.Configurations.Add(new CategoryGroupMap());
            modelBuilder.Configurations.Add(new CategoryOrganizationUnitMap());
            modelBuilder.Configurations.Add(new FirmAddressMap());
            modelBuilder.Configurations.Add(new FirmAddressServiceMap());
            modelBuilder.Configurations.Add(new FirmContactMap());
            modelBuilder.Configurations.Add(new FirmMap());
            modelBuilder.Configurations.Add(new TerritoryMap());
            modelBuilder.Configurations.Add(new BusinessEntityInstanceMap());
            modelBuilder.Configurations.Add(new BusinessEntityPropertyInstanceMap());
            modelBuilder.Configurations.Add(new DictionaryEntityInstanceMap());
            modelBuilder.Configurations.Add(new DictionaryEntityPropertyInstanceMap());
            modelBuilder.Configurations.Add(new AdditionalFirmServiceMap());
            modelBuilder.Configurations.Add(new BuildingMap());
            modelBuilder.Configurations.Add(new CardRelationMap());
            modelBuilder.Configurations.Add(new CityPhoneZoneMap());
            modelBuilder.Configurations.Add(new DepCardMap());
            modelBuilder.Configurations.Add(new ExportFailedEntityMap());
            modelBuilder.Configurations.Add(new ExportFlowCardExtensions_CardCommercialMap());
            modelBuilder.Configurations.Add(new ExportFlowDeliveryData_LetterSendRequestMap());
            modelBuilder.Configurations.Add(new ExportFlowFinancialData_ClientMap());
            modelBuilder.Configurations.Add(new ExportFlowFinancialData_LegalEntityMap());
            modelBuilder.Configurations.Add(new ExportFlowNomenclatures_NomenclatureElementMap());
            modelBuilder.Configurations.Add(new ExportFlowNomenclatures_NomenclatureElementRelationMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_AdvMaterialMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_DenialReasonMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_InvoiceMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_OrderMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_ResourceMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_ThemeMap());
            modelBuilder.Configurations.Add(new ExportFlowOrders_ThemeBranchMap());
            modelBuilder.Configurations.Add(new ExportFlowPriceLists_PriceListMap());
            modelBuilder.Configurations.Add(new ExportFlowPriceLists_PriceListPositionMap());
            modelBuilder.Configurations.Add(new HotClientRequestMap());
            modelBuilder.Configurations.Add(new ImportedFirmAddressMap());
            modelBuilder.Configurations.Add(new ReferenceMap());
            modelBuilder.Configurations.Add(new ReferenceItemMap());
            modelBuilder.Configurations.Add(new BirthdayCongratulationMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new FunctionalPrivilegeDepthMap());
            modelBuilder.Configurations.Add(new PrivilegeMap());
            modelBuilder.Configurations.Add(new RolePrivilegeMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserEntityMap());
            modelBuilder.Configurations.Add(new UserOrganizationUnitMap());
            modelBuilder.Configurations.Add(new UserProfileMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserTerritoryMap());
            modelBuilder.Configurations.Add(new ActionsHistoryMap());
            modelBuilder.Configurations.Add(new ActionsHistoryDetailMap());
            modelBuilder.Configurations.Add(new BusinessOperationServiceMap());
            modelBuilder.Configurations.Add(new FileMap());
            modelBuilder.Configurations.Add(new LocalMessageMap());
            modelBuilder.Configurations.Add(new MessageTypeMap());
            modelBuilder.Configurations.Add(new NoteMap());
            modelBuilder.Configurations.Add(new NotificationAddressMap());
            modelBuilder.Configurations.Add(new NotificationEmailMap());
            modelBuilder.Configurations.Add(new NotificationEmailsAttachmentMap());
            modelBuilder.Configurations.Add(new NotificationEmailsCcMap());
            modelBuilder.Configurations.Add(new NotificationEmailsToMap());
            modelBuilder.Configurations.Add(new NotificationProcessingMap());
            modelBuilder.Configurations.Add(new OperationMap());
            modelBuilder.Configurations.Add(new PerformedBusinessOperationMap());
            modelBuilder.Configurations.Add(new PerformedOperationFinalProcessingMap());
            modelBuilder.Configurations.Add(new PerformedOperationPrimaryProcessingMap());
            modelBuilder.Configurations.Add(new TimeZoneMap());
            modelBuilder.Configurations.Add(new vwOrganizationUnitMap());
            modelBuilder.Configurations.Add(new vwSecurityAcceleratorMap());
            modelBuilder.Configurations.Add(new vwUsersDescendantMap());
            modelBuilder.Configurations.Add(new vwUserTerritoriesOrganizationUnitMap());

            return  modelBuilder.Build(connection).Compile();
        }
    }
}