using System;

using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class ErmTableNames
    {
        public static readonly SchemaQualifiedObjectName Advertisements = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Advertisements");

        public static readonly SchemaQualifiedObjectName AdvertisementElementTemplates = new SchemaQualifiedObjectName(
            ErmSchemas.Billing,
            "AdvertisementElementTemplates");

        public static readonly SchemaQualifiedObjectName AdvertisementTemplates = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AdvertisementTemplates");

        public static readonly SchemaQualifiedObjectName OperationTypes = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OperationTypes");

        public static readonly SchemaQualifiedObjectName Orders = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Orders");

        public static readonly SchemaQualifiedObjectName OrderPositions = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderPositions");

        public static readonly SchemaQualifiedObjectName OrderProcessingRequests = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderProcessingRequests");

        public static readonly SchemaQualifiedObjectName OrderProcessingRequestMessages = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                        "OrderProcessingRequestMessages");

        public static readonly SchemaQualifiedObjectName OrderReleaseTotals = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderReleaseTotals");

        public static readonly SchemaQualifiedObjectName OrderFiles = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderFiles");

        public static readonly SchemaQualifiedObjectName Bills = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Bills");

        public static readonly SchemaQualifiedObjectName LegalPersonProfiles = new SchemaQualifiedObjectName(ErmSchemas.Billing, "LegalPersonProfiles");

        public static readonly SchemaQualifiedObjectName LegalPersons = new SchemaQualifiedObjectName(ErmSchemas.Billing, "LegalPersons");

        public static readonly SchemaQualifiedObjectName Bargains = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Bargains");

        public static readonly SchemaQualifiedObjectName Contacts = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Contacts");

        public static readonly SchemaQualifiedObjectName Clients = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Clients");

        public static readonly SchemaQualifiedObjectName DenormalizedClientLinks = new SchemaQualifiedObjectName(ErmSchemas.Billing, "DenormalizedClientLinks");

        public static readonly SchemaQualifiedObjectName ClientLinks = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ClientLinks");

        public static readonly SchemaQualifiedObjectName FirmDeals = new SchemaQualifiedObjectName(ErmSchemas.Billing, "FirmDeals");

        public static readonly SchemaQualifiedObjectName LegalPersonDeals = new SchemaQualifiedObjectName(ErmSchemas.Billing, "LegalPersonDeals");

        public static readonly SchemaQualifiedObjectName Deals = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Deals");

        public static readonly SchemaQualifiedObjectName Limits = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Limits");

        public static readonly SchemaQualifiedObjectName Migrations = new SchemaQualifiedObjectName(ErmSchemas.Shared, "Migrations");

        public static readonly SchemaQualifiedObjectName OrganizationUnits = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrganizationUnits");

        public static readonly SchemaQualifiedObjectName AccountDetails = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AccountDetails");

        public static readonly SchemaQualifiedObjectName Currencies = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Currencies");

        public static readonly SchemaQualifiedObjectName CurrencyRates = new SchemaQualifiedObjectName(ErmSchemas.Billing, "CurrencyRates");

        public static readonly SchemaQualifiedObjectName Prices = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Prices");

        public static readonly SchemaQualifiedObjectName Countries = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Countries");

        public static readonly SchemaQualifiedObjectName ContributionTypes = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ContributionTypes");

        public static readonly SchemaQualifiedObjectName BranchOffices = new SchemaQualifiedObjectName(ErmSchemas.Billing, "BranchOffices");

        public static readonly SchemaQualifiedObjectName BargainTypes = new SchemaQualifiedObjectName(ErmSchemas.Billing, "BargainTypes");

        public static readonly SchemaQualifiedObjectName Platforms = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Platforms");

        public static readonly SchemaQualifiedObjectName AdsTemplatesAdsElementTemplates = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                         "AdsTemplatesAdsElementTemplates");

        public static readonly SchemaQualifiedObjectName OrderValidationRuleGroups = new SchemaQualifiedObjectName(
            ErmSchemas.Billing,
            "OrderValidationRuleGroups");

        public static readonly SchemaQualifiedObjectName OrderValidationRuleGroupDetails = new SchemaQualifiedObjectName(
            ErmSchemas.Billing,
            "OrderValidationRuleGroupDetails");

        public static readonly SchemaQualifiedObjectName OrderValidationResults = new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderValidationResults");

        public static readonly SchemaQualifiedObjectName Positions = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Positions");

        public static readonly SchemaQualifiedObjectName PositionCategories = new SchemaQualifiedObjectName(ErmSchemas.Billing, "PositionCategories");

        public static readonly SchemaQualifiedObjectName PositionChildren = new SchemaQualifiedObjectName(ErmSchemas.Billing, "PositionChildren");
        
        public static readonly SchemaQualifiedObjectName PricePositions = new SchemaQualifiedObjectName(ErmSchemas.Billing, "PricePositions");

        public static readonly SchemaQualifiedObjectName Projects = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Projects");

        public static readonly SchemaQualifiedObjectName ReleaseInfos = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReleaseInfos");

        public static readonly SchemaQualifiedObjectName ReleaseValidationResults = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReleaseValidationResults");

        public static readonly SchemaQualifiedObjectName ReleasesWithdrawalsPositions = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                      "ReleasesWithdrawalsPositions");

        public static readonly SchemaQualifiedObjectName ReleasesWithdrawals = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReleasesWithdrawals");

        public static readonly SchemaQualifiedObjectName WithdrawalInfos = new SchemaQualifiedObjectName(ErmSchemas.Billing, "WithdrawalInfos");

        public static readonly SchemaQualifiedObjectName BranchOfficeOrganizationUnits = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                       "BranchOfficeOrganizationUnits");

        public static readonly SchemaQualifiedObjectName UserProfiles = new SchemaQualifiedObjectName(ErmSchemas.Security, "UserProfiles");

        public static readonly SchemaQualifiedObjectName Users = new SchemaQualifiedObjectName(ErmSchemas.Security, "Users");

        public static readonly SchemaQualifiedObjectName RolePrivileges = new SchemaQualifiedObjectName(ErmSchemas.Security, "RolePrivileges");

        public static readonly SchemaQualifiedObjectName UserRoles = new SchemaQualifiedObjectName(ErmSchemas.Security, "UserRoles");

        public static readonly SchemaQualifiedObjectName UserEntities = new SchemaQualifiedObjectName(ErmSchemas.Security, "UserEntities");

        public static readonly SchemaQualifiedObjectName FunctionalPrivilegeDepths = new SchemaQualifiedObjectName(ErmSchemas.Security,
                                                                                                                   "FunctionalPrivilegeDepths");

        public static readonly SchemaQualifiedObjectName TimeZones = new SchemaQualifiedObjectName(ErmSchemas.Shared, "TimeZones");

        public static readonly SchemaQualifiedObjectName Files = new SchemaQualifiedObjectName(ErmSchemas.Shared, "Files");
        
        public static readonly SchemaQualifiedObjectName Operations = new SchemaQualifiedObjectName(ErmSchemas.Shared, "Operations");

        public static readonly SchemaQualifiedObjectName ActionsHistory = new SchemaQualifiedObjectName(ErmSchemas.Shared, "ActionsHistory");

        public static readonly SchemaQualifiedObjectName ActionsHistoryDetails = new SchemaQualifiedObjectName(ErmSchemas.Shared, "ActionsHistoryDetails");

        public static readonly SchemaQualifiedObjectName ActivityInstances = new SchemaQualifiedObjectName(ErmSchemas.Activity, "ActivityInstances");

        public static readonly SchemaQualifiedObjectName ActivityPropertyInstances = new SchemaQualifiedObjectName(ErmSchemas.Activity,
                                                                                                                   "ActivityPropertyInstances");

        public static readonly SchemaQualifiedObjectName LocalMessages = new SchemaQualifiedObjectName(ErmSchemas.Shared, "LocalMessages");

        public static readonly SchemaQualifiedObjectName CrmReplicationInfo = new SchemaQualifiedObjectName(ErmSchemas.Shared, "CrmReplicationInfo");

        public static readonly SchemaQualifiedObjectName CrmReplicationDetails = new SchemaQualifiedObjectName(ErmSchemas.Shared, "CrmReplicationDetails");
        
        public static readonly SchemaQualifiedObjectName AfterSaleServiceActivities = new SchemaQualifiedObjectName(
            ErmSchemas.Billing,
            "AfterSaleServiceActivities");

        public static readonly SchemaQualifiedObjectName ThemeCategories = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ThemeCategories");

        public static readonly SchemaQualifiedObjectName ThemeOrganizationUnits = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ThemeOrganizationUnits");

        public static readonly SchemaQualifiedObjectName Themes = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Themes");

        public static readonly SchemaQualifiedObjectName ThemeTemplates = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ThemeTemplates");

        public static readonly SchemaQualifiedObjectName Categories = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "Categories");

        public static readonly SchemaQualifiedObjectName CategoryOrganizationUnits = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory,
                                                                                                                   "CategoryOrganizationUnits");

        public static readonly SchemaQualifiedObjectName CategoryGroups = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "CategoryGroups");

        public static readonly SchemaQualifiedObjectName Territories = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "Territories");

        public static readonly SchemaQualifiedObjectName OrderPositionAdvertisement =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "OrderPositionAdvertisement");

        public static readonly SchemaQualifiedObjectName Firms = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "Firms");

        public static readonly SchemaQualifiedObjectName FirmAddresses = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "FirmAddresses");

        public static readonly SchemaQualifiedObjectName CategoryFirmAddresses = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory,
                                                                                                               "CategoryFirmAddresses");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName FirmAddressServices = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "FirmAddressServices");

        [Obsolete("заменена на PerformedBusinessOperations")]
        public static readonly SchemaQualifiedObjectName BusinessOperations = new SchemaQualifiedObjectName(ErmSchemas.Shared, "BusinessOperations");

        [Obsolete("заменена на ServiceBusExportedBusinessOperations")]
        public static readonly SchemaQualifiedObjectName ExportBusinessOperations = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                  "ExportBusinessOperations");

        public static readonly SchemaQualifiedObjectName BusinessOperationServices = new SchemaQualifiedObjectName(ErmSchemas.Shared,
                                                                                                                   "BusinessOperationServices");

        public static readonly SchemaQualifiedObjectName PerformedBusinessOperations = new SchemaQualifiedObjectName(ErmSchemas.Shared,
                                                                                                                     "PerformedBusinessOperations");

        [Obsolete("заменена на семь других таблиц")]
        public static readonly SchemaQualifiedObjectName ServiceBusExportedBusinessOperations = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                              "ServiceBusExportedBusinessOperations");

        public static readonly SchemaQualifiedObjectName HotClientRequests = new SchemaQualifiedObjectName(ErmSchemas.Integration, "HotClientRequests");

        public static readonly SchemaQualifiedObjectName CardRelations = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CardRelations");

        public static readonly SchemaQualifiedObjectName ExportFlowOrdersAdvMaterial = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                     "ExportFlowOrders_AdvMaterial");

        public static readonly SchemaQualifiedObjectName ExportFlowOrdersResource = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                  "ExportFlowOrders_Resource");
        
        public static readonly SchemaQualifiedObjectName ExportFlowOrdersTheme = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFlowOrders_Theme");
        
        public static readonly SchemaQualifiedObjectName ExportFlowOrdersThemeBranch = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                     "ExportFlowOrders_ThemeBranch");
        
        public static readonly SchemaQualifiedObjectName ExportFlowFinancialDataLegalEntity = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                            "ExportFlowFinancialData_LegalEntity");
        
        public static readonly SchemaQualifiedObjectName ExportFlowOrdersOrder = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFlowOrders_Order");

        public static readonly SchemaQualifiedObjectName ExportFlowOrdersInvoice = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                 "ExportFlowOrders_Invoice");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName ExportToMsCrmHotClients = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                 "ExportToMsCrm_HotClients");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName ExportFlowCardExtensionsCardCommercial = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                                "ExportFlowCardExtensions_CardCommercial");
        
        public static readonly SchemaQualifiedObjectName ExportFlowOrdersDenialReason = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                      "ExportFlowOrders_DenialReason");

        public static readonly SchemaQualifiedObjectName ImportedFirmAddresses = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ImportedFirmAddresses");

        public static readonly SchemaQualifiedObjectName ExportFailedEntities = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFailedEntities");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName ExportSessions = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportSessions");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName ExportSessionDetails = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportSessionDetails");

        public static readonly SchemaQualifiedObjectName Departments = new SchemaQualifiedObjectName(ErmSchemas.Security, "Departments");

        public static readonly SchemaQualifiedObjectName Privileges = new SchemaQualifiedObjectName(ErmSchemas.Security, "Privileges");

        public static readonly SchemaQualifiedObjectName Roles = new SchemaQualifiedObjectName(ErmSchemas.Security, "Roles");

        public static readonly SchemaQualifiedObjectName MessageTypes = new SchemaQualifiedObjectName(ErmSchemas.Shared, "MessageTypes");

        public static readonly SchemaQualifiedObjectName DenialReasons = new SchemaQualifiedObjectName(ErmSchemas.Billing, "DenialReasons");

        public static readonly SchemaQualifiedObjectName AdvertisementElements = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AdvertisementElements");

        public static readonly SchemaQualifiedObjectName Accounts = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Accounts");

        public static readonly SchemaQualifiedObjectName Locks = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Locks");

        public static readonly SchemaQualifiedObjectName LockDetails = new SchemaQualifiedObjectName(ErmSchemas.Billing, "LockDetails");

        public static readonly SchemaQualifiedObjectName BargainFiles = new SchemaQualifiedObjectName(ErmSchemas.Billing, "BargainFiles");

        public static readonly SchemaQualifiedObjectName AssociatedPositions = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AssociatedPositions");

        public static readonly SchemaQualifiedObjectName AssociatedPositionsGroups = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                   "AssociatedPositionsGroups");

        public static readonly SchemaQualifiedObjectName DeniedPositions = new SchemaQualifiedObjectName(ErmSchemas.Billing, "DeniedPositions");

        public static readonly SchemaQualifiedObjectName FirmContacts = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "FirmContacts");

        public static readonly SchemaQualifiedObjectName SalesModelCategoryRestrictions = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "SalesModelCategoryRestrictions");

        public static readonly SchemaQualifiedObjectName Reference = new SchemaQualifiedObjectName(ErmSchemas.Integration, "Reference");

        public static readonly SchemaQualifiedObjectName ReferenceItems = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ReferenceItems");

        public static readonly SchemaQualifiedObjectName UserOrganizationUnits = new SchemaQualifiedObjectName(ErmSchemas.Security, "UserOrganizationUnits");

        public static readonly SchemaQualifiedObjectName UserTerritories = new SchemaQualifiedObjectName(ErmSchemas.Security, "UserTerritories");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName AdditionalFirmServices = new SchemaQualifiedObjectName(ErmSchemas.Integration, "AdditionalFirmServices");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName RegionalAdvertisingSharings = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                     "RegionalAdvertisingSharings");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName OrdersRegionalAdvertisingSharings = new SchemaQualifiedObjectName(ErmSchemas.Billing,
                                                                                                                           "OrdersRegionalAdvertisingSharings");

        public static readonly SchemaQualifiedObjectName Notes = new SchemaQualifiedObjectName(ErmSchemas.Shared, "Notes");

        public static readonly SchemaQualifiedObjectName NotificationEmails = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmails");

        public static readonly SchemaQualifiedObjectName NotificationAddresses = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationAddresses");

        public static readonly SchemaQualifiedObjectName NotificationEmailsTo = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsTo");

        public static readonly SchemaQualifiedObjectName NotificationProcessings = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationProcessings");

        public static readonly SchemaQualifiedObjectName PrintFormTemplates = new SchemaQualifiedObjectName(ErmSchemas.Billing, "PrintFormTemplates");

        public static readonly SchemaQualifiedObjectName NotificationEmailsAttachments = new SchemaQualifiedObjectName(ErmSchemas.Shared,
                                                                                                                       "NotificationEmailsAttachments");

        public static readonly SchemaQualifiedObjectName NotificationEmailsCc = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsCc");

        [Obsolete("Таблица была удалена")]
        public static readonly SchemaQualifiedObjectName XmlSchemas = new SchemaQualifiedObjectName(ErmSchemas.Shared, "XmlSchemas");

        public static readonly SchemaQualifiedObjectName Buildings = new SchemaQualifiedObjectName(ErmSchemas.Integration, "Buildings");

        public static readonly SchemaQualifiedObjectName ExportFlowFinancialDataClient = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                       "ExportFlowFinancialData_Client");
        
        public static readonly SchemaQualifiedObjectName CityPhoneZone = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CityPhoneZone");

        public static readonly SchemaQualifiedObjectName DepCards = new SchemaQualifiedObjectName(ErmSchemas.Integration, "DepCards");

        public static readonly SchemaQualifiedObjectName DictionaryEntityInstances = new SchemaQualifiedObjectName(ErmSchemas.DynamicStorage,
                                                                                                                   "DictionaryEntityInstances");

        public static readonly SchemaQualifiedObjectName DictionaryEntityPropertyInstances = new SchemaQualifiedObjectName(ErmSchemas.DynamicStorage,
                                                                                                                           "DictionaryEntityPropertyInstances");

        public static readonly SchemaQualifiedObjectName BusinessEntityInstances = new SchemaQualifiedObjectName(ErmSchemas.DynamicStorage,
                                                                                                                 "BusinessEntityInstances");

        public static readonly SchemaQualifiedObjectName BusinessEntityPropertyInstances = new SchemaQualifiedObjectName(ErmSchemas.DynamicStorage,
                                                                                                                         "BusinessEntityPropertyInstances");

        public static readonly SchemaQualifiedObjectName ExportFlowPriceListsPriceList = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                       "ExportFlowPriceLists_PriceList");

        [Obsolete("Use ExportFlowPriceListsPriceListPosition")]
        public static readonly SchemaQualifiedObjectName ExportFlowPriceListsPricePosition = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                           "ExportFlowPriceLists_PricePosition");

        public static readonly SchemaQualifiedObjectName ExportFlowPriceListsPriceListPosition = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                               "ExportFlowPriceLists_PriceListPosition");

        public static readonly SchemaQualifiedObjectName Charges = new SchemaQualifiedObjectName(ErmSchemas.Billing, "Charges");
        public static readonly SchemaQualifiedObjectName ChargesHistory = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ChargesHistory");
        public static readonly SchemaQualifiedObjectName ExportFlowNomenclaturesNomenclatureElement = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFlowNomenclatures_NomenclatureElement");
        public static readonly SchemaQualifiedObjectName ExportFlowNomenclaturesNomenclatureElementRelation = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFlowNomenclatures_NomenclatureElementRelation");

        public static readonly SchemaQualifiedObjectName PerformedOperationPrimaryProcessings = new SchemaQualifiedObjectName(ErmSchemas.Shared, "PerformedOperationPrimaryProcessings");

        public static readonly SchemaQualifiedObjectName PerformedOperationFinalProcessings = new SchemaQualifiedObjectName(ErmSchemas.Shared, "PerformedOperationFinalProcessings");

        public static readonly SchemaQualifiedObjectName ExportFlowDeliveryDataLetterSendRequest = new SchemaQualifiedObjectName(ErmSchemas.Integration,
                                                                                                                                 "ExportFlowDeliveryData_LetterSendRequest");

        public static readonly SchemaQualifiedObjectName BirthdayCongratulations = new SchemaQualifiedObjectName(ErmSchemas.Journal,
                                                                                                                 "BirthdayCongratulations");

        public static readonly SchemaQualifiedObjectName AdvertisementElementStatus = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AdvertisementElementStatuses");
        public static readonly SchemaQualifiedObjectName AdvertisementElementDenialReason = new SchemaQualifiedObjectName(ErmSchemas.Billing, "AdvertisementElementDenialReasons");
        public static readonly SchemaQualifiedObjectName OrderValidationCacheEntries = new SchemaQualifiedObjectName(ErmSchemas.Shared, "OrderValidationCacheEntries");

        public static readonly SchemaQualifiedObjectName ExportFlowFinancialDataDebitsInfoInitial = new SchemaQualifiedObjectName(ErmSchemas.Integration, "ExportFlowFinancialData_DebitsInfoInitial");

        public static readonly SchemaQualifiedObjectName TelephonyAddresses = new SchemaQualifiedObjectName(ErmSchemas.Security, "TelephonyAddresses");
    }
}
