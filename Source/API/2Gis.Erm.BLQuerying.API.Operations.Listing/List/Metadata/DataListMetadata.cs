using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public struct DataListInfo
    {
        public string DefaultFilter { get; set; }
        public string ExtendedInfo { get; set; }
    }

    public static class DataListMetadata
    {
        private static readonly Dictionary<Tuple<EntityName, string>, DataListInfo> DataListMap = new Dictionary<Tuple<EntityName, string>, DataListInfo>
        {
            { Tuple.Create(EntityName.Account, "DListAccounts"), new DataListInfo { DefaultFilter = "IsActive && !IsDeleted" } },
            // Мои лицевые счета с отрицательным балансом
            { Tuple.Create(EntityName.Account, "DListMyAccountsWithNegativeBalance"), new DataListInfo {DefaultFilter = "IsActive && !IsDeleted && OwnerCode={systemuserid} && Balance<0" } },
            // Все лицевые счета по по филиалу
            { Tuple.Create(EntityName.Account, "DListAccountsAtMyBranch"), new DataListInfo {DefaultFilter = "IsActive && !IsDeleted && LegalPerson.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },
            // Лицевые счета моих подчиненных с отрицательным балансом
            { Tuple.Create(EntityName.Account, "DListAccountsWithNegativeBalanceForSubordinates"), new DataListInfo {DefaultFilter = "IsActive && !IsDeleted && Balance<0", ExtendedInfo = "ForSubordinates=true"} },
            // Мои лицевые счета с размещающимися БЗ
            { Tuple.Create(EntityName.Account, "DListMyAccountsWithHostedOrders"), new DataListInfo {DefaultFilter = "IsActive && !IsDeleted && OwnerCode={systemuserid}", ExtendedInfo = "WithHostedOrders=true"} },
            // Лицевые счета моих подчиненных с размещающимися БЗ
            { Tuple.Create(EntityName.Account, "DListAccountsWithHostedOrdersForSubordinates"), new DataListInfo {DefaultFilter = "IsActive && !IsDeleted", ExtendedInfo = "WithHostedOrders=true;ForSubordinates=true"} },

            { Tuple.Create(EntityName.AccountDetail, "DListAccountDetails"), new DataListInfo {DefaultFilter = "!IsDeleted" } },
            { Tuple.Create(EntityName.AccountDetail, "DListAccountDetailsWithDeletedOperations"), new DataListInfo {DefaultFilter = "!IsDeleted" } },

            { Tuple.Create(EntityName.ActivityInstance, "DListAllActivities"), new DataListInfo {DefaultFilter = "!IsDeleted" } },
            // Активные действия
            { Tuple.Create(EntityName.ActivityInstance, "DListActiveActivities"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive && Status=1" } },
            // Закрытые действия
            { Tuple.Create(EntityName.ActivityInstance, "DListInactiveActivities"), new DataListInfo {DefaultFilter = "IsDeleted || !IsActive || Status=2 || Status=3" } },
            // Мои действия
            { Tuple.Create(EntityName.ActivityInstance, "DListMyActivities"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive && OwnerCode={systemuserid}" } },
            // Действия по моим подчиненным
            { Tuple.Create(EntityName.ActivityInstance, "DListActivitiesForSubordinates"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive", ExtendedInfo = "ForSubordinates=true" } },
            // Мои завершенные действия
            { Tuple.Create(EntityName.ActivityInstance, "DListMyCompletedActivities"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive && OwnerCode={systemuserid}  &&  Status=2" } },
            // Завершенные действия по моим подчиненным
            { Tuple.Create(EntityName.ActivityInstance, "DListCompletedActivitiesForSubordinates"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive  &&  Status=2", ExtendedInfo = "ForSubordinates=true" } },
            // Мои запланированные действия
            { Tuple.Create(EntityName.ActivityInstance, "DListMyActivitiesInProgress"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive && OwnerCode={systemuserid}  &&  Status=1" } },
            // Запланированные действия по моим подчиненным
            { Tuple.Create(EntityName.ActivityInstance, "DListActivitiesInProgressForSubordinates"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive  &&  Status=1", ExtendedInfo = "ForSubordinates=true" } },
            // Мои запланированные действия на сегодня
            { Tuple.Create(EntityName.ActivityInstance, "DListMyActivitiesInProgressForToday"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive && OwnerCode={systemuserid}  &&  Status=1", ExtendedInfo = "ForToday=true" } },
            // Действия по теплым клиентам
            { Tuple.Create(EntityName.ActivityInstance, "DListActivitiesForWarmClients"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive  &&  TaskType=1  &&  ScheduledEnd >= DateTime.Today  &&  Status=1" } },
            // Просроченные действия по теплым клиентам
            { Tuple.Create(EntityName.ActivityInstance, "DListOverdueActivitiesForWarmClients"), new DataListInfo {DefaultFilter = "!IsDeleted && IsActive  &&  TaskType=1  &&  ScheduledEnd < DateTime.Today  &&  Status=1" } },

            { Tuple.Create(EntityName.AdditionalFirmService, "AdditionalFirmServices"), new DataListInfo() },

            { Tuple.Create(EntityName.AdsTemplatesAdsElementTemplate, "DListAdsTemplatesAdsElementTemplate"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.AdvertisementTemplate, "DListAdvertisementTemplate"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },
            { Tuple.Create(EntityName.AdvertisementTemplate, "DListAdvertisementTemplateDeleted"), new DataListInfo {DefaultFilter = "IsDeleted=true" } },

            { Tuple.Create(EntityName.AdvertisementElementTemplate, "DListAdvertisementElementTemplate"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },
            { Tuple.Create(EntityName.AdvertisementElementTemplate, "DListAdvertisementElementTemplateDeleted"), new DataListInfo {DefaultFilter = "IsDeleted=true" } },

            { Tuple.Create(EntityName.Advertisement, "DListActiveAdvertisement"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },
            { Tuple.Create(EntityName.Advertisement, "DListInactiveAdvertisement"), new DataListInfo {DefaultFilter = "IsDeleted=true" } },

            { Tuple.Create(EntityName.AdvertisementElement, "DListAdvertisementElement"), new DataListInfo() },

            { Tuple.Create(EntityName.AssociatedPosition, "DListAssociatedPosition"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.AssociatedPosition, "DListAssociatedPositionInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.AssociatedPositionsGroup, "DListAssociatedPositionsGroup"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.AssociatedPositionsGroup, "DListAssociatedPositionsGroupInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.Bank, "DListBanks"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.Bargain, "DListBargains"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            // Мои договоры
            { Tuple.Create(EntityName.Bargain, "DListMyBargains"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid}" } },

            { Tuple.Create(EntityName.BargainFile, "DListBargainFiles"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.BargainType, "DListActiveBargainTypes"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.BargainType, "DListInactiveBargainTypes"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.Bill, "DListBill"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.BranchOffice, "DListBranchOfficeActive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.BranchOffice, "DListBranchOfficeInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.BranchOfficeOrganizationUnit, "DListBranchOfficeOrganizationUnit"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && it.OrganizationUnit.IsDeleted=false && it.BranchOffice.IsDeleted=false" } },
            { Tuple.Create(EntityName.BranchOfficeOrganizationUnit, "DListBranchOfficeOrganizationUnitInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false && it.OrganizationUnit.IsDeleted=false && it.BranchOffice.IsDeleted=false" } },

            { Tuple.Create(EntityName.Category, "DListActiveCategories"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Category, "DListInactiveCategories"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.CategoryFirmAddress, "DListActiveCategoryFirmAddresses"), new DataListInfo {DefaultFilter = "IsActive" } },
            { Tuple.Create(EntityName.CategoryFirmAddress, "DListInactiveCategoryFirmAddresses"), new DataListInfo {DefaultFilter = "!IsActive" } },

            { Tuple.Create(EntityName.CategoryGroup, "DListActiveCategoryGroups"), new DataListInfo {DefaultFilter = "!it.IsDeleted  &&  IsActive" } },
            { Tuple.Create(EntityName.CategoryGroup, "DListAllCategoryGroups"), new DataListInfo {DefaultFilter = "!it.IsDeleted" } },

            { Tuple.Create(EntityName.CategoryOrganizationUnit, "DListCategoryOrganizationUnits"), new DataListInfo {DefaultFilter = "it.IsDeleted=false && it.Category.IsDeleted=false" } },

            { Tuple.Create(EntityName.Client, "DListClients"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            // Мои клиенты
            { Tuple.Create(EntityName.Client, "DListMyClients"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid}" } },
            // Мои клиенты, созданные сегодня
            { Tuple.Create(EntityName.Client, "DListMyClientsCreatedToday"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && CreatedOn>=Datetime.Today" } },
            // Клиенты на моей территории
            { Tuple.Create(EntityName.Client, "DListClientsOnMyTerritory"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && it.Territory.UserTerritoriesOrganizationUnits.Any(UserId = {systemuserid})" } },
            // Мои клиенты с дебиторской задолженностью
            { Tuple.Create(EntityName.Client, "DListMyClientsWithDebt"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && LegalPersons.Any(IsDeleted=false && IsActive=true && Accounts.Any(IsDeleted=false && IsActive=true && Balance<{balancedebtborder}))" } },
            // Клиенты в резерве на моей территории
            { Tuple.Create(EntityName.Client, "DListReservedClientsOnMyTerritory"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={reserveuserid} && it.Territory.UserTerritoriesOrganizationUnits.Any(UserId = {systemuserid})" } },
            // Мои теплые клиенты
            { Tuple.Create(EntityName.Client, "DListMyWarmClients"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && InformationSource=5" } },
            // Клиенты, у которых есть заказы с типом Бартер
            { Tuple.Create(EntityName.Client, "DListClientsWithBarter"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && LegalPersons.Any(IsDeleted=false && IsActive=true && Orders.Any(IsDeleted=false && IsActive=true && (OrderType=4||OrderType=5||OrderType=6)))" } },
            // Мои клиенты без ЛПР
            { Tuple.Create(EntityName.Client, "DListMyClientsWithoutMakeDecisionContacts"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && !Contacts.Any(IsDeleted=false && IsActive=true && IsFired=false && AccountRole=200002)" } },
            // Клиенты по моим подчиненным
            { Tuple.Create(EntityName.Client, "DListClientsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo="ForSubordinates=true" } },
            // Теплые клиенты моих подчиненных
            { Tuple.Create(EntityName.Client, "DListWarmClientsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && InformationSource=5", ExtendedInfo="ForSubordinates=true" } },
            // Клиенты моих подчиненных, у которых есть заказы с типом Бартер
            { Tuple.Create(EntityName.Client, "DListClientsWithBarterForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && LegalPersons.Any(IsDeleted=false && IsActive=true && Orders.Any(IsDeleted=false && IsActive=true && (OrderType=4||OrderType=5||OrderType=6)))", ExtendedInfo="ForSubordinates=true" } },
            // Клиенты моих подчиненных без ЛПР
            { Tuple.Create(EntityName.Client, "DListClientsWithoutMakeDecisionContactsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && !Contacts.Any(IsDeleted=false && IsActive=true && IsFired=false && AccountRole=200002)", ExtendedInfo="ForSubordinates=true" } },
            // Региональные клиенты моих подчиненных
            { Tuple.Create(EntityName.Client, "DListRegionalClientsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && LegalPersons.Any(IsDeleted=false && IsActive=true && Orders.Any(IsDeleted=false && IsActive=true && SourceOrganizationUnitId!=DestOrganizationUnitId))", ExtendedInfo="ForSubordinates=true" } },
            // Клиенты моих подчиненных, у которых несколько открытых сделок
            { Tuple.Create(EntityName.Client, "DListClientsWithSeveralOpenDealsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Deals.Count(IsDeleted=false && IsActive=true)>1", ExtendedInfo="ForSubordinates=true" } },
            // Все клиенты по филиалу
            { Tuple.Create(EntityName.Client, "DListClientsAtMyBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && it.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId = {systemuserid})" } },
            // Клиенты моих подчиненных с дебиторской задолженностью
            { Tuple.Create(EntityName.Client, "DListClientsWithDebtForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && LegalPersons.Any(IsDeleted=false && IsActive=true && Accounts.Any(IsDeleted=false && IsActive=true && Balance<{balancedebtborder}))", ExtendedInfo="ForSubordinates=true" } },
            // Клиенты, с которыми была только одна встреча
            { Tuple.Create(EntityName.Client, "DListClientsWithOnly1Appointment"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo="With1Appointment=true" } },
            // Тёплые клиенты
            { Tuple.Create(EntityName.Client, "DListClientsWithWarmClientTask"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo="WarmClientTask=true;Outdated=false" } },
            // Тёплые клиенты с просроченной задачей
            { Tuple.Create(EntityName.Client, "DListClientsWithOutdatedWarmClientTask"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo="WarmClientTask=true;Outdated=true" } },

            { Tuple.Create(EntityName.Commune, "DListCommunes"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.Contact, "DListActiveContacts"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && IsFired=false" } },
            { Tuple.Create(EntityName.Contact, "DListFiredContacts"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && IsFired=true" } },
            // Мои контактные лица
            { Tuple.Create(EntityName.Contact, "DListMyContacts"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && IsFired=false && OwnerCode={systemuserid}" } },

            { Tuple.Create(EntityName.ContributionType, "DListActiveContributionType"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.ContributionType, "DListInactiveContributionType"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.Country, "DListCountries"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },
            
            { Tuple.Create(EntityName.Currency, "DListCurrencies"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Currency, "DListCurrenciesInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.CurrencyRate, "DListCurrencyRates"), new DataListInfo() },

            { Tuple.Create(EntityName.Deal, "DListActiveDeals"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Deal, "DListInactiveDeals"), new DataListInfo {DefaultFilter = "IsActive=false||IsDeleted=true" } },
            // Мои сделки
            { Tuple.Create(EntityName.Deal, "DListMyDeals"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid}" } },
            // Мои закрытые сделки
            { Tuple.Create(EntityName.Deal, "DListMyInactiveDeals"), new DataListInfo {DefaultFilter = "(IsActive=false||IsDeleted=true) && OwnerCode={systemuserid}" } },
            // Мои бартерные сделки
            { Tuple.Create(EntityName.Deal, "DListMyBarterDeals"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && Orders.Any(OrderType == 4 || OrderType == 5 || OrderType == 6)" } },
            // Все сделки по филиалу
            { Tuple.Create(EntityName.Deal, "DListDealsAtBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },
            // Сделки моих подчиненных
            { Tuple.Create(EntityName.Deal, "DListDealsForSubordinates"), new DataListInfo { ExtendedInfo="ForSubordinates=true" } },

            { Tuple.Create(EntityName.DeniedPosition, "DListDeniedPosition"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.DeniedPosition, "DListDeniedPositionInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            
            { Tuple.Create(EntityName.Department, "DListDepartment"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Department, "DListInactiveDepartment"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            
            { Tuple.Create(EntityName.Firm, "DListActiveFirms"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false" } },
            { Tuple.Create(EntityName.Firm, "DListInactiveFirms"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=true" } },
            // Мои фирмы
            { Tuple.Create(EntityName.Firm, "DListMyFirms"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid}" } },
            // Фирмы в резерве на моей территории
            { Tuple.Create(EntityName.Firm, "DListReservedFirmsOnMyTerritories"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false && (LastDisqualifyTime=null||((DateTime.Now.Month - LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - LastDisqualifyTime.Value.Year))>2) && OwnerCode={reserveuserid} && Territory.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },
            // Новые фирмы моей территории
            { Tuple.Create(EntityName.Firm, "DListNewFirmsOnMyTerritories"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false && Territory.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})", ExtendedInfo="CreatedInCurrentMonth=true" } },
            // Все фирмы по филиалу
            { Tuple.Create(EntityName.Firm, "DListFirmsAtMyBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId = {systemuserid})" } },
            // Все активные фирмы по филиалу
            { Tuple.Create(EntityName.Firm, "DListActiveFirmsAtMyBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId = {systemuserid})" } },
            // Фирмы моих подчиненных
            { Tuple.Create(EntityName.Firm, "DListFirmsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo = "ForSubordinates=true"} },
            // Фирмы с заказами с типом Самореклама
            { Tuple.Create(EntityName.Firm, "DListFirmsWithSelfAds"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Orders.Any(IsDeleted=false && IsActive=true && OrderType=2)" } },

            { Tuple.Create(EntityName.FirmAddress, "DListActiveFirmAddresses"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false" } },
            { Tuple.Create(EntityName.FirmAddress, "DListInactiveFirmAddresses"), new DataListInfo {DefaultFilter = "IsDeleted=false && (IsActive=false || ClosedForAscertainment=true)" } },

            { Tuple.Create(EntityName.FirmContact, "DListFirmContacts"), new DataListInfo() },

            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersons"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            // Мои юридические лица
            { Tuple.Create(EntityName.LegalPerson, "DListMyLegalPersons"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid}" } },
            // Мои юридические лица с дебиторской задолженностью
            { Tuple.Create(EntityName.LegalPerson, "DListMyLegalPersonsWithDebt"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode={systemuserid} && Accounts.Any(IsActive=true && IsDeleted=false && Balance<{balancedebtborder})" } },
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsWithMyOrders"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && OwnerCode!={systemuserid} && Orders.Any(IsDeleted=false && IsActive=true && OwnerCode={systemuserid})" } },
            // Все юридические лица по филиалу
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsAtMyBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },
            // Юридические лица моих подчиненных
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true", ExtendedInfo="ForSubordinates=true" } },
            // Юридические лица моих подчиненных с дебиторской задолженностью
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsWithDebtForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Accounts.Any(IsActive=true && IsDeleted=false && Balance<{balancedebtborder})", ExtendedInfo="ForSubordinates=true" } },
            // Юридические лица по филиалу с дебиторской задолженностью
            { Tuple.Create(EntityName.LegalPerson, "DListLegalPersonsWithDebtAtMyBranch"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && Accounts.Any(IsActive=true && IsDeleted=false && Balance<{balancedebtborder}) && Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },

            { Tuple.Create(EntityName.Limit, "DListLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false" } },
            { Tuple.Create(EntityName.Limit, "DListLimitsInactive"), new DataListInfo {DefaultFilter = "IsActive=false && IsDeleted=false" } },
            // Мои открытые лимиты
            { Tuple.Create(EntityName.Limit, "DListMyOpenedLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && Status=1", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Мои отклоненные лимиты
            { Tuple.Create(EntityName.Limit, "DListMyRejectedLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && Status=3", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Мои одобренные лимиты
            { Tuple.Create(EntityName.Limit, "DListMyApprovedLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && Status=2", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Лимиты по моим подчиненным
            { Tuple.Create(EntityName.Limit, "DListLimitsForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false", ExtendedInfo="ForSubordinates=true;useNextMonthForStartPeriodDate=true" } },
            // Лимиты, требующие моего одобрения
            { Tuple.Create(EntityName.Limit, "DListLimitsForApprove"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && Status=1", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Одобренные мною лимиты
            { Tuple.Create(EntityName.Limit, "DListApprovedByMeLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && Status=2", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Отклоненные мною лимиты
            { Tuple.Create(EntityName.Limit, "DListRejectedByMeLimits"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && Status=3", ExtendedInfo="useNextMonthForStartPeriodDate=true" } },
            // Лимиты по филиалу
            { Tuple.Create(EntityName.Limit, "DListLimitsAtMyBranch"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && Account.LegalPerson.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(UserId={systemuserid})" } },

            { Tuple.Create(EntityName.LocalMessage, "DListLocalMessageAll"), new DataListInfo() },
            { Tuple.Create(EntityName.LocalMessage, "DListLocalMessageActive"), new DataListInfo {DefaultFilter = "(Status=1 || Status=2 || Status=3)  &&  it.MessageType.ReceiverSystem=1" } },
            { Tuple.Create(EntityName.LocalMessage, "DListLocalMessageProcessed"), new DataListInfo {DefaultFilter = "Status=4  &&  it.MessageType.ReceiverSystem=1" } },
            { Tuple.Create(EntityName.LocalMessage, "DListLocalMessageFailed"), new DataListInfo {DefaultFilter = "Status=5  &&  it.MessageType.ReceiverSystem=1" } },
            { Tuple.Create(EntityName.LocalMessage, "DListLocalMessageOutbox"), new DataListInfo {DefaultFilter = "it.MessageType.SenderSystem=1" } },

            { Tuple.Create(EntityName.Lock, "DListActiveLocks"), new DataListInfo {DefaultFilter = "IsActive=True" } },
            { Tuple.Create(EntityName.Lock, "DListNotActiveLocks"), new DataListInfo {DefaultFilter = "IsActive=False" } },

            { Tuple.Create(EntityName.LegalPersonProfile, "DListLegalPersonProfiles"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.LockDetail, "DListActiveLockDetails"), new DataListInfo {DefaultFilter = "IsActive=True" } },
            { Tuple.Create(EntityName.LockDetail, "DListNotActiveLockDetails"), new DataListInfo {DefaultFilter = "IsActive=False" } },

            { Tuple.Create(EntityName.Operation, "DListOperations"), new DataListInfo() },
            { Tuple.Create(EntityName.Operation, "DListOperationsAfterSaleService"), new DataListInfo {DefaultFilter = "Type=150" } },

            { Tuple.Create(EntityName.OrderPosition, "DListOrderPositions"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.OperationType, "DListOperationTypes"), new DataListInfo() },

            { Tuple.Create(EntityName.OrderPositionAdvertisement, "DListOrderPositionAdvertisements"), new DataListInfo() },

            { Tuple.Create(EntityName.Order, "DListActiveOrders"), new DataListInfo {DefaultFilter = "(IsActive=true && IsDeleted=false && WorkflowStepId!=6) || (IsDeleted=false && (WorkflowStepId=6||WorkflowStepId=4) && EndDistributionDateFact>DateTime.Now)" } },
            { Tuple.Create(EntityName.Order, "DListInactiveOrders"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && WorkflowStepId==6" } },
            { Tuple.Create(EntityName.Order, "DListRejectedOrders"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            { Tuple.Create(EntityName.Order, "DListAllOrders"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },
            // Все мои активные заказы
            { Tuple.Create(EntityName.Order, "DListMyActiveOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid}" } },
            // Мои заказы на расторжении
            { Tuple.Create(EntityName.Order, "DListMyOrdersOnTermination"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && WorkflowStepId=4" } },
            // Мои заказы в статусе На утверждении
            { Tuple.Create(EntityName.Order, "DListMyOrdersOnApproval"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && WorkflowStepId=2" } },
            // Мои неактивные (заказы закрытые отказом)
            { Tuple.Create(EntityName.Order, "DListMyTerminatedOrders"), new DataListInfo {DefaultFilter = "IsDeleted=false && OwnerCode={systemuserid} && IsTerminated=true" } },
            // Мои заказы, у которых отсутствуют подписанные документы
            { Tuple.Create(EntityName.Order, "DListMyOrdersWithDocumentsDebt"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && HasDocumentsDebt=1" } },
            // Все заказы моих подчиненных
            { Tuple.Create(EntityName.Order, "DListOrdersForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false", ExtendedInfo="ForSubordinates=true" } },
            // Неактивные (закрытые отказом) заказы моих подчиненных
            { Tuple.Create(EntityName.Order, "DListTerminatedOrdersForSubordinates"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsTerminated=true", ExtendedInfo="ForSubordinates=true" } },
            // Все мои заказы с типом Самореклама
            { Tuple.Create(EntityName.Order, "DListMySelfAdsOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && OrderType=2" } },
            // Все мои заказы с типом Бартер
            { Tuple.Create(EntityName.Order, "DListMyBarterOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && (OrderType=4||OrderType=5||OrderType=6)" } },
            // Мои новые заказы
            { Tuple.Create(EntityName.Order, "DListMyNewOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && (WorkflowStepId=1||WorkflowStepId=2||WorkflowStepId=3||WorkflowStepId=5) && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))<=2 && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))>0" } },
            // Новые заказы моих подчиненных
            { Tuple.Create(EntityName.Order, "DListNewOrdersForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=1||WorkflowStepId=2||WorkflowStepId=3||WorkflowStepId=5) && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))<=2 && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))>0", ExtendedInfo="ForSubordinates=true" } },
            // Заказы, требующие продления
            { Tuple.Create(EntityName.Order, "DListOrdersToProlongate"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=5)", ExtendedInfo="useCurrentMonthForEndDistributionDateFact=true" } },
            // Заказы моих подчиненных, требующие продления
            { Tuple.Create(EntityName.Order, "DListOrdersToProlongateForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=5)", ExtendedInfo="ForSubordinates=true;useCurrentMonthForEndDistributionDateFact=true" } },
            // Отклоненные заказы моих подчиненных
            { Tuple.Create(EntityName.Order, "DListRejectedOrdersForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && WorkflowStepId=3", ExtendedInfo="ForSubordinates=true" } },
            // Мои заказы в ближайший выпуск
            { Tuple.Create(EntityName.Order, "DListMyOrdersToNextEdition"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && OwnerCode={systemuserid} && (WorkflowStepId=1||WorkflowStepId=2||WorkflowStepId=3||WorkflowStepId=5)", ExtendedInfo="ForNextEdition=true" } },
            // Заказы моих подчиненных в ближайший выпуск
            { Tuple.Create(EntityName.Order, "DListOrdersToNextEditionForSubordinates"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=1||WorkflowStepId=2||WorkflowStepId=3||WorkflowStepId=5)", ExtendedInfo="ForSubordinates=true;ForNextEdition=true" } },
            // Все отклоненные мною БЗ
            { Tuple.Create(EntityName.Order, "DListRejectedByMeOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && WorkflowStepId=3 && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))<=2 && ((BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (BeginDistributionDate.Year - DateTime.Now.Year))>0" } },
            // Заказы, требующие моего одобрения
            { Tuple.Create(EntityName.Order, "DListOrdersOnApprovalForMe"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && WorkflowStepId=2" } },
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            { Tuple.Create(EntityName.Order, "DListApprovedOrdersWithoutAdvertisement"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && WorkflowStepId=5", ExtendedInfo="WithoutAdvertisement=true" } },
            // Заказы в выпуск следующего месяца закрытые отказом
            { Tuple.Create(EntityName.Order, "DListTerminatedOrdersForNextMonthEdition"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsTerminated=true", ExtendedInfo="ForNextMonthEdition=true" } },
            // Неподписанные БЗ за текущий выпуск
            { Tuple.Create(EntityName.Order, "DListOrdersWithDocumentsDebtForNextMonth"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && HasDocumentsDebt=1 && (WorkflowStepId=1||WorkflowStepId=2||WorkflowStepId=3||WorkflowStepId=5)", ExtendedInfo="ForNextEdition=true" } },
            // Список технических расторжений
            { Tuple.Create(EntityName.Order, "DListTechnicalTerminatedOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=4||IsTerminated=true) && TerminationReason=12" } },
            // Список действительных расторжений
            { Tuple.Create(EntityName.Order, "DListNonTechnicalTerminatedOrders"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && (WorkflowStepId=4||IsTerminated=true) && TerminationReason!=12 && TerminationReason>0" } },
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            { Tuple.Create(EntityName.Order, "DListRejectedByMeOrdersOnRegistration"), new DataListInfo {DefaultFilter = "IsActive=true && IsDeleted=false && InspectorCode={systemuserid} && WorkflowStepId=1", ExtendedInfo="RejectedByMe=true" } },

            { Tuple.Create(EntityName.OrderProcessingRequest, "DListOrderProcessingRequest"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.OrderFile, "DListOrderFiles"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.OrganizationUnit, "DListOrganizationUnitActive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.OrganizationUnit, "DListOrganizationUnitInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            { Tuple.Create(EntityName.OrganizationUnit, "DListOrganizationUnitActiveMovedToErm"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ErmLaunchDate!=null" } },
            { Tuple.Create(EntityName.OrganizationUnit, "DListOrganizationUnitActiveMovedToIR"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && InfoRussiaLaunchDate!=null" } },

            { Tuple.Create(EntityName.Platform, "DListPlatform"), new DataListInfo {DefaultFilter = "" } },

            { Tuple.Create(EntityName.Position, "DListPositions"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Position, "DListPositionInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.PositionCategory, "DListPositionCategory"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.PositionChildren, "DListPositionChildren"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.PricePosition, "DListPricePositions"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.PricePosition, "DListPricePositionsInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.Price, "DListPrices"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.Price, "DListPricesInactive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },

            { Tuple.Create(EntityName.PrintFormTemplate, "DListPrintFormTemplates"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.Project, "DListProjects"), new DataListInfo {DefaultFilter = "IsActive=true" } },

            { Tuple.Create(EntityName.RegionalAdvertisingSharing, "DListRegionalAdvertisingSharing"), new DataListInfo() },

            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfo"), new DataListInfo() },
            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfoInProgress"), new DataListInfo {DefaultFilter = "Status=1" } },
            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfoSuccessed"), new DataListInfo {DefaultFilter = "Status=2" } },
            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfoFailed"), new DataListInfo {DefaultFilter = "Status=3" } },
            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfoBeta"), new DataListInfo {DefaultFilter = "IsBeta=true" } },
            { Tuple.Create(EntityName.ReleaseInfo, "DListReleaseInfoFinal"), new DataListInfo {DefaultFilter = "IsBeta=false" } },
            
            { Tuple.Create(EntityName.Role, "DListRole"), new DataListInfo() },

            { Tuple.Create(EntityName.Theme, "DListThemeActive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.ThemeTemplate, "DListThemeTemplateActive"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.ThemeOrganizationUnit, "DListUserOrganizationUnit"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.ThemeCategory, "DListThemeCategory"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.TimeZone, "DListTimeZones"), new DataListInfo() },

            { Tuple.Create(EntityName.Territory, "DListTerritoryActive"), new DataListInfo {DefaultFilter = "IsActive=true" } },
            { Tuple.Create(EntityName.Territory, "DListTerritoryInactive"), new DataListInfo {DefaultFilter = "IsActive=false" } },

            { Tuple.Create(EntityName.User, "DListUser"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },
            { Tuple.Create(EntityName.User, "DListInactiveUser"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=false" } },
            { Tuple.Create(EntityName.User, "DListUserWithRole"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true" } },

            { Tuple.Create(EntityName.UserRole, "DListUserRole"), new DataListInfo() },

            { Tuple.Create(EntityName.UserOrganizationUnit, "DListUserOrganizationUnit"), new DataListInfo() },
            { Tuple.Create(EntityName.UserOrganizationUnit, "DListUsersInOrganizationUnit"), new DataListInfo() },

            { Tuple.Create(EntityName.UserTerritory, "DListUserTerritory"), new DataListInfo {DefaultFilter = "IsDeleted=false" } },

            { Tuple.Create(EntityName.WithdrawalInfo, "DListWithdrawalInfo"), new DataListInfo() },

            // Специфичные для Кипра
            { Tuple.Create(EntityName.Firm, "DListReservedFirmsLefkosia"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false && (LastDisqualifyTime=null||((DateTime.Now.Month - LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - LastDisqualifyTime.Value.Year))>2) && OwnerCode={reserveuserid} && OrganizationUnitId=133"} },
            { Tuple.Create(EntityName.Firm, "DListReservedFirmsLemesos"), new DataListInfo {DefaultFilter = "IsDeleted=false && IsActive=true && ClosedForAscertainment=false && (LastDisqualifyTime=null||((DateTime.Now.Month - LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - LastDisqualifyTime.Value.Year))>2) && OwnerCode={reserveuserid} && OrganizationUnitId=122"} },
        };

        public static bool TryGetDataListInfo(EntityName entityName, string nameLocaleResourceId, out DataListInfo dataListInfo)
        {
            // первый фильтр является фильтром по умолчанию
            if (string.IsNullOrEmpty(nameLocaleResourceId))
            {
                dataListInfo = DataListMap.Where(x => x.Key.Item1 == entityName).Select(x => x.Value).First();
                return true;
            }

            return DataListMap.TryGetValue(Tuple.Create(entityName, nameLocaleResourceId), out dataListInfo);
        }
    }
}
