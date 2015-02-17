using System.Collections.Generic;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class ExtendedInfoMetadata
    {
        private static readonly Dictionary<string, string> ExtendedInfoMap = new Dictionary<string, string>
        {
            {"DListAccounts", "ActiveAndNotDeleted=true"},
            // Мои лицевые счета с отрицательным балансом
            {"DListMyAccountsWithNegativeBalance", "ActiveAndNotDeleted=true;ForMe=true;NegativeBalance=true" },
            // Все лицевые счета по по филиалу
            {"DListAccountsAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true" },
            // Лицевые счета моих подчиненных с отрицательным балансом
            {"DListAccountsWithNegativeBalanceForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;NegativeBalance=true" },
            // Мои лицевые счета с размещающимися БЗ
            {"DListMyAccountsWithHostedOrders", "ActiveAndNotDeleted=true;WithHostedOrders=true;ForMe=true" },
            // Лицевые счета моих подчиненных с размещающимися БЗ
            {"DListAccountsWithHostedOrdersForSubordinates", "ActiveAndNotDeleted=true;WithHostedOrders=true;ForSubordinates=true" },

            {"DListAccountDetails", "Deleted=false"},
            {"DListAccountDetailsWithDeletedOperations", "Deleted=true"},

            {"DListAllActivities", "ActiveAndNotDeleted=true"},
            // Активные действия
            {"DListActiveActivities", "ActiveAndNotDeleted=true;InProgress=true"},
            // Закрытые действия
            {"DListInactiveActivities", "ActiveAndNotDeleted=true;Completed=true;"},
            // Мои действия
            {"DListMyActivities", "ActiveAndNotDeleted=true;ForMe=true" },
            //Мои открытые планы
            {"DListMyOpenedActivities","ActiveAndNotDeleted=true;InProgress=true;ForMe=true"},
            // Действия по моим подчиненным
            {"DListActivitiesForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },
            // Мои завершенные действия
            {"DListMyCompletedActivities", "ActiveAndNotDeleted=true;Completed=true;ForMe=true" },
            // Завершенные действия по моим подчиненным
            {"DListCompletedActivitiesForSubordinates", "ActiveAndNotDeleted=true;Completed=true;ForSubordinates=true" },
            // Мои запланированные действия
            {"DListMyActivitiesInProgress", "ActiveAndNotDeleted=true;InProgress=true;ForMe=true" },
            // Запланированные действия по моим подчиненным
            {"DListActivitiesInProgressForSubordinates", "ActiveAndNotDeleted=true;InProgress=true;ForSubordinates=true" },
            // Мои запланированные действия на сегодня
            {"DListMyActivitiesInProgressForToday", "ActiveAndNotDeleted=true;InProgress=true;ForToday=true;ForMe=true" },
            // Действия по горячим клиентам
            {"DListActivitiesForWarmClients", "ActiveAndNotDeleted=true;InProgress=true;WarmClient=true" },
            // Просроченные действия по горячим клиентам
            {"DListOverdueActivitiesForWarmClients", "ActiveAndNotDeleted=true;InProgress=true;Expired=true;WarmClient=true" },

            {"DListAdsTemplatesAdsElementTemplate", "NotDeleted=true"},

            {"DListAdvertisementElementDenialReasonForEdit", "ActiveOrChecked=true"},

            {"DListDenialReason", "Active=true"},
            {"DListInactiveDenialReason", "Active=false"},

            {"DListAdvertisementTemplate", "Deleted=false"},
            {"DListAdvertisementTemplateDeleted", "Deleted=true"},

            {"DListAdvertisementElementTemplate", "Deleted=false"},
            {"DListAdvertisementElementTemplateDeleted", "Deleted=true"},

            {"DListActiveAdvertisement", "Deleted=false"},
            {"DListInactiveAdvertisement", "Deleted=true"},

            {"DListAssociatedPosition", "ActiveAndNotDeleted=true"},
            {"DListAssociatedPositionInactive", "NotActiveAndNotDeleted=true"},

            {"DListAssociatedPositionsGroup", "ActiveAndNotDeleted=true"},
            {"DListAssociatedPositionsGroupInactive", "NotActiveAndNotDeleted=true"},

            {"DListBargains", "ActiveAndNotDeleted=true"},
            {"DListInactiveBargains", "NotActiveAndNotDeleted=true"},
            // Мои договоры
            {"DListMyBargains", "ActiveAndNotDeleted=true;ForMe=true" },

            {"DListBargainFiles", "NotDeleted=true"},

            {"DListActiveBargainTypes", "ActiveAndNotDeleted=true"},
            {"DListInactiveBargainTypes", "NotActiveAndNotDeleted=true"},

            {"DListBill", "ActiveAndNotDeleted=true"},

            {"DListBranchOfficeActive", "ActiveAndNotDeleted=true"},
            {"DListBranchOfficeInactive", "NotActiveAndNotDeleted=true"},

            {"DListBranchOfficeOrganizationUnit", "ActiveAndNotDeleted=true;ParentsNotDeleted=true"},
            {"DListBranchOfficeOrganizationUnitInactive", "NotActiveAndNotDeleted=true;ParentsNotDeleted=true"},

            {"DListActiveCategories", "ActiveAndNotDeleted=true"},
            {"DListInactiveCategories", "NotActiveAndNotDeleted=true"},

            {"DListActiveCategoryFirmAddresses", "ActiveBusinessMeaning=true"},
            {"DListInactiveCategoryFirmAddresses", "InactiveBusinessMeaning=true"},

            {"DListActiveCategoryGroups", "ActiveAndNotDeleted=true"},
            {"DListAllCategoryGroups", "NotDeleted=true"},

            {"DListClients", "ActiveAndNotDeleted=true"},
            // Мои клиенты
            {"DListMyClients", "ActiveAndNotDeleted=true;ForMe=true" },
            // Мои клиенты, созданные сегодня
            {"DListMyClientsCreatedToday", "ActiveAndNotDeleted=true;ForToday=true;ForMe=true" },
            // Клиенты на моей территории
            {"DListClientsOnMyTerritory", "ActiveAndNotDeleted=true;MyTerritory=true" },
            // Мои клиенты с дебиторской задолженностью
            {"DListMyClientsWithDebt", "ActiveAndNotDeleted=true;ForMe=true;WithDebt=true" },
            // Клиенты в резерве на моей территории
            {"DListReservedClientsOnMyTerritory", "ActiveAndNotDeleted=true;MyTerritory=true;ForReserve=true" },
            // Мои горячие клиенты
            {"DListMyWarmClients", "ActiveAndNotDeleted=true;ForMe=true;Warm=true" },
            // Клиенты, у которых есть заказы с типом Бартер
            {"DListClientsWithBarter", "ActiveAndNotDeleted=true;WithBarterOrders=true" },
            // Мои клиенты без ЛПР
            {"DListMyClientsWithoutMakeDecisionContacts", "ActiveAndNotDeleted=true;ForMe=true;NoMakingDecisions=true" },
            // Клиенты по моим подчиненным
            {"DListClientsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },
            // Горячие клиенты моих подчиненных
            {"DListWarmClientsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;Warm=true" },
            // Клиенты моих подчиненных, у которых есть заказы с типом Бартер
            {"DListClientsWithBarterForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;WithBarterOrders=true" },
            // Клиенты моих подчиненных без ЛПР
            {"DListClientsWithoutMakeDecisionContactsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;NoMakingDecisions=true" },
            // Региональные клиенты моих подчиненных
            {"DListRegionalClientsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;IsRegional=true" },
            // Клиенты моих подчиненных, у которых несколько открытых сделок
            {"DListClientsWithSeveralOpenDealsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;MinDealCount=1" },
            // Все клиенты по филиалу
            {"DListClientsAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true" },
            // Клиенты моих подчиненных с дебиторской задолженностью
            {"DListClientsWithDebtForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;WithDebt=true" },
            // Клиенты, с которыми была только одна встреча
            {"DListClientsWithOnly1Appointment", "ActiveAndNotDeleted=true;With1Appointment=true" },
            // Тёплые клиенты
            {"DListClientsWithWarmClientTask", "ActiveAndNotDeleted=true;WarmClientTask=true;Outdated=false" },
            // Тёплые клиенты с просроченной задачей
            {"DListClientsWithOutdatedWarmClientTask", "ActiveAndNotDeleted=true;WarmClientTask=true;Outdated=true" },

            {"DListActiveContacts", "ActiveAndNotDeleted=true;Fired=false" },
            {"DListFiredContacts", "ActiveAndNotDeleted=true;Fired=true" },

            // Все доступные для связывания клиенты
            {"DListClientsAvailableForLinking", "ActiveAndNotDeleted=true" },
            
            // Дочерние клиенты
            {"DListClientLinks", "IsDeleted=false;ClientLinks=true" },
            // Родительские клиенты
            {"DListClientLinksMaster", "IsDeleted=false;ClientLinksMaster=true" },
            // Удалённые связи
            {"DListClientLinksDeleted", "IsDeleted=true;ClientLinksDeleted=true" },

            // Мои контактные лица
            {"DListMyContacts", "ActiveAndNotDeleted=true;Fired=false;ForMe=true" },

            {"DListActiveContributionType", "ActiveAndNotDeleted=true"},
            {"DListInactiveContributionType", "NotActiveAndNotDeleted=true"},

            {"DListCategoryOrganizationUnits", "NotDeletedAndParentsNotDeleted=true"},

            {"DListCountries", "NotDeleted=true"},

            {"DListCurrencies", "ActiveAndNotDeleted=true"},
            {"DListCurrenciesInactive", "NotActiveAndNotDeleted=true"},

            {"DListActiveDeals", "ActiveAndNotDeleted=true"},
            {"DListInactiveDeals", "NotActiveAndNotDeleted=true"},
            // Мои сделки
            {"DListMyDeals", "ActiveAndNotDeleted=true;ForMe=true" },
            // Мои закрытые сделки
            {"DListMyInactiveDeals", "NotActiveAndNotDeleted=true;ForMe=true" },
            // Мои бартерные сделки
            {"DListMyBarterDeals", "ActiveAndNotDeleted=true;ForMe=true;WithBarterOrders=true" },
            // Все сделки по филиалу
            {"DListDealsAtBranch", "ActiveAndNotDeleted=true;MyBranch=true" },
            // Сделки моих подчиненных
            {"DListDealsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },

            {"DListDeniedPosition", "ActiveAndNotDeleted=true"},
            {"DListDeniedPositionInactive", "NotActiveAndNotDeleted=true"},

            {"DListDepartment", "ActiveAndNotDeleted=true"},
            {"DListInactiveDepartment", "NotActiveAndNotDeleted=true"},

            {"DListActiveFirms", "ActiveBusinessMeaning=true"},
            {"DListInactiveFirms", "InactiveBusinessMeaning=true"},
            // Мои фирмы
            {"DListMyFirms", "ActiveAndNotDeleted=true;ForMe=true" },
            // Фирмы в резерве на моей территории
            {"DListReservedFirmsOnMyTerritories", "ActiveBusinessMeaning=true;QualifyTimeLastYear=true;MyTerritory=true;ForReserve=true" },
            // Новые фирмы моей территории
            {"DListNewFirmsOnMyTerritories", "ActiveBusinessMeaning=true;CreatedInCurrentMonth=true;MyTerritory=true" },
            // Все фирмы по филиалу
            {"DListFirmsAtMyBranch", "NotDeleted=true;MyBranch=true" },
            // Все активные фирмы по филиалу
            {"DListActiveFirmsAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true" },
            // Фирмы моих подчиненных
            {"DListFirmsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },
            // Фирмы с заказами с типом Самореклама
            {"DListFirmsWithSelfAds", "ActiveAndNotDeleted=true;WithSelfAdsOrders=true" },
            {"DListActiveFirmsToAppend", "ActiveAndNotDeleted=true" },

            {"DListActiveFirmAddresses", "ActiveBusinessMeaning=true"},
            {"DListInactiveFirmAddresses", "InactiveBusinessMeaning=true"},

            {"DListActiveFirmsForDeal", "Deleted=false"},
            {"DListInactiveFirmsForDeal", "Deleted=true"},

            {"DListLegalPersons", "ActiveAndNotDeleted=true"},
            {"DListLegalPersonsInactive", "NotActiveAndNotDeleted=true"},
            // Мои юридические лица
            {"DListMyLegalPersons", "ActiveAndNotDeleted=true;ForMe=true" },
            // Мои юридические лица с дебиторской задолженностью
            {"DListMyLegalPersonsWithDebt", "ActiveAndNotDeleted=true;ForMe=true;WithDebt=true" },
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            {"DListLegalPersonsWithMyOrders", "ActiveAndNotDeleted=true;HasMyOrders=true;ForMe=false" },
            // Все юридические лица по филиалу
            {"DListLegalPersonsAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true" },
            // Юридические лица моих подчиненных
            {"DListLegalPersonsForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },
            // Юридические лица моих подчиненных с дебиторской задолженностью
            {"DListLegalPersonsWithDebtForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true;WithDebt=true" },
            // Юридические лица по филиалу с дебиторской задолженностью
            {"DListLegalPersonsWithDebtAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true;WithDebt=true" },

            {"DListLegalPersonProfiles", "ActiveAndNotDeleted=true"},

            {"DListActiveLegalPersonsForDeal", "NotDeleted=true"},

            {"DListLimits", "ActiveAndNotDeleted=true"},
            {"DListLimitsInactive", "NotActiveAndNotDeleted=true"},
            // Мои открытые лимиты
            {"DListMyOpenedLimits", "ActiveAndNotDeleted=true;Opened=true;useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Мои отклоненные лимиты
            {"DListMyRejectedLimits", "ActiveAndNotDeleted=true;Rejected=true;useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Мои одобренные лимиты
            {"DListMyApprovedLimits", "ActiveAndNotDeleted=true;Approved=true;useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Лимиты по моим подчиненным
            {"DListLimitsForSubordinates", "ActiveAndNotDeleted=true;useNextMonthForStartPeriodDate=true;ForSubordinates=true" },
            // Лимиты, требующие моего одобрения
            {"DListLimitsForApprove", "ActiveAndNotDeleted=true;Opened=true;useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Одобренные мною лимиты
            {"DListApprovedByMeLimits", "ActiveAndNotDeleted=true;Approved=true;useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Отклоненные мною лимиты
            {"DListRejectedByMeLimits", "ActiveAndNotDeleted=true;Rejected=true;useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Лимиты по филиалу
            {"DListLimitsAtMyBranch", "ActiveAndNotDeleted=true;MyBranch=true" },

            {"DListLocalMessageActive", "ActiveBusinessMeaning=true;ToErm=true"},
            {"DListLocalMessageProcessed", "Processed=true;ToErm=true"},
            {"DListLocalMessageFailed", "Failed=true;ToErm=true"},
            {"DListLocalMessageOutbox", "FromErm=true"},

            {"DListActiveLocks", "Active=true"},
            {"DListNotActiveLocks", "Active=false"},
            {"DListActiveLockDetails", "Active=true"},
            {"DListNotActiveLockDetails", "Active=false"},

            {"DListActiveOrders", "ActiveBusinessMeaning=true"},
            {"DListInactiveOrders", "ActiveAndNotDeleted=true;Archive=true"},
            {"DListRejectedOrders", "NotActiveAndNotDeleted=true"},
            {"DListAllOrders", "NotDeleted=true"},
            // Все мои активные заказы
            {"DListMyActiveOrders", "ActiveAndNotDeleted=true;ForMe=true" },
            // Мои заказы на расторжении
            {"DListMyOrdersOnTermination", "ActiveAndNotDeleted=true;OnTermination=true;ForMe=true" },
            // Мои заказы в статусе На утверждении
            {"DListMyOrdersOnApproval", "ActiveAndNotDeleted=true;OnApproval=true;ForMe=true" },
            // Мои неактивные (заказы закрытые отказом)
            {"DListMyTerminatedOrders", "NotActiveAndNotDeleted=true;ForMe=true" },
            // Мои заказы, у которых отсутствуют подписанные документы
            {"DListMyOrdersWithDocumentsDebt", "ActiveAndNotDeleted=true;Absent=true;ForMe=true" },
            // Все заказы моих подчиненных
            {"DListOrdersForSubordinates", "ActiveAndNotDeleted=true;ForSubordinates=true" },
            // Неактивные (закрытые отказом) заказы моих подчиненных
            {"DListTerminatedOrdersForSubordinates", "NotActiveAndNotDeleted=true;ForSubordinates=true" },
            // Все мои заказы с типом Самореклама
            {"DListMySelfAdsOrders", "ActiveAndNotDeleted=true;SelfAds=true;ForMe=true" },
            // Все мои заказы с типом Бартер
            {"DListMyBarterOrders", "ActiveAndNotDeleted=true;Barter=true;ForMe=true" },
            // Мои новые заказы
            {"DListMyNewOrders", "ActiveAndNotDeleted=true;AllActiveStatuses=true;New=true;ForMe=true" },
            // Новые заказы моих подчиненных
            {"DListNewOrdersForSubordinates", "ActiveAndNotDeleted=true;AllActiveStatuses=true;New=true;ForSubordinates=true" },
            // Заказы, требующие продления
            {"DListOrdersToProlongate", "ActiveAndNotDeleted=true;Approved=true;useCurrentMonthForEndDistributionDateFact=true" },
            // Заказы моих подчиненных, требующие продления
            {"DListOrdersToProlongateForSubordinates", "ActiveAndNotDeleted=true;Approved=true;ForSubordinates=true;useCurrentMonthForEndDistributionDateFact=true" },
            // Отклоненные заказы моих подчиненных
            {"DListRejectedOrdersForSubordinates", "ActiveAndNotDeleted=true;Rejected=true;ForSubordinates=true" },
            // Мои заказы в ближайший выпуск
            {"DListMyOrdersToNextEdition", "ActiveAndNotDeleted=true;AllActiveStatuses=true;ForNextEdition=true;ForMe=true" },
            // Заказы моих подчиненных в ближайший выпуск
            {"DListOrdersToNextEditionForSubordinates", "ActiveAndNotDeleted=true;AllActiveStatuses=true;ForSubordinates=true;ForNextEdition=true" },
            // Все отклоненные мною БЗ
            {"DListRejectedByMeOrders", "ActiveAndNotDeleted=true;Rejected=true;New=true;MyInspection=true" },
            // Заказы, требующие моего одобрения
            {"DListOrdersOnApprovalForMe", "ActiveAndNotDeleted=true;OnApproval=true;MyInspection=true" },
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            {"DListApprovedOrdersWithoutAdvertisement", "ActiveAndNotDeleted=true;OnApproval=true;WithoutAdvertisement=true" },
            // Заказы в выпуск следующего месяца закрытые отказом
            {"DListTerminatedOrdersForNextMonthEdition", "ActiveAndNotDeleted=true;ForNextMonthEdition=true" },
            // Неподписанные БЗ за текущий выпуск
            {"DListOrdersWithDocumentsDebtForNextMonth", "ActiveAndNotDeleted=true;AllActiveStatuses=true;Absent=true;ForNextEdition=true" },
            // Список технических расторжений
            {"DListTechnicalTerminatedOrders", "ActiveAndNotDeleted=true;TechnicalTerminated=true"},
            // Список действительных расторжений
            {"DListNonTechnicalTerminatedOrders", "ActiveAndNotDeleted=true;NonTechnicalTerminated=true"},
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            {"DListRejectedByMeOrdersOnRegistration", "ActiveAndNotDeleted=true;OnRegistration=true;RejectedByMe=true;MyInspection=true" },
            {"DListOrdersFast", "NotDeleted=true"},

            {"DListOrderProcessingRequest", "NotDeleted=true"},

            {"DListOrderFiles", "NotDeleted=true"},
            {"DListOrderPositions", "NotDeleted=true"},

            {"DListOrganizationUnitActive", "ActiveAndNotDeleted=true"}, 
            {"DListOrganizationUnitInactive", "NotActiveAndNotDeleted=true"}, 
            {"DListOrganizationUnitActiveMovedToErm", "ActiveAndNotDeleted=true;UseErm=true"}, 
            {"DListOrganizationUnitActiveMovedToIR", "ActiveAndNotDeleted=true;UseIR=true"}, 

            {"DListPositions", "ActiveAndNotDeleted=true"},
            {"DListPositionInactive", "NotActiveAndNotDeleted=true"},

            {"DListPositionChildren", "ActiveAndNotDeleted=true"},

            {"DListPositionCategory", "NotDeleted=true"},

            {"DListPrices", "ActiveAndNotDeleted=true"},
            {"DListPricesInactive", "NotActiveAndNotDeleted=true"},

            {"DListPricePositions", "ActiveAndNotDeleted=true"},
            {"DListPricePositionsInactive", "NotActiveAndNotDeleted=true"},

            {"DListReleaseInfoSuccessed", "Success=true"},
            {"DListReleaseInfoFailed", "Error=true"},
            {"DListReleaseInfoInProgress", "InProgress=true"},
            {"DListReleaseInfoBeta", "Beta=true"},
            {"DListReleaseInfoFinal", "Beta=false"},

            {"DListPrintFormTemplates", "NotDeleted=true"},
            {"DListProjects", "Active=true"},

            {"DListTerritoryActive", "Active=true"},
            {"DListTerritoryInactive", "Active=false"},

            {"DListUser", "ActiveAndNotDeleted=true"},
            {"DListUserWithRole", "ActiveAndNotDeleted=true"},
            {"DListInactiveUser", "NotActiveAndNotDeleted=true"},

            {"DListUserTerritory", "TerritoryActiveAndNotDeleted=true"},

            {"DListUserOrganizationUnit", "ParentsActiveAndNotDeleted=true"},
            {"DListUsersInOrganizationUnit", "ParentsActiveAndNotDeleted=true"},

            {"DListThemeActive", "ActiveAndNotDeleted=true"},
            {"DListThemeTemplateActive", "ActiveAndNotDeleted=true"},
            {"DListThemeOrganizationUnit", "ActiveAndNotDeleted=true"},
            {"DListThemeCategory", "NotDeleted=true"},

            {"AdditionalFirmServices", ""},
            {"DListAdvertisementElementDenialReason", ""},
            {"DListAdvertisementElement", ""},
            {"DListCurrencyRates", ""},
            {"DListFirmContacts", ""},
            {"DListLocalMessageAll", ""},
            {"DListOperations", ""},
            {"DListOperationTypes", ""},
            {"DListOrderPositionAdvertisements", ""},
            {"DListPlatform", ""},
            {"DListReleaseInfo", ""},
            {"DListRole", ""},
            {"DListTimeZones", ""},
            {"DListUserRole", ""},
            {"DListWithdrawalInfo", ""},
        };

        public static void RegisterExtendedInfo(string filterName, string extendedInfo)
        {
            if (!ExtendedInfoMap.ContainsKey(filterName))
            {
                ExtendedInfoMap.Add(filterName, extendedInfo);
            }
        }

        public static bool TryGetExtendedInfo(string filterName, out string extendedInfo)
        {
            return ExtendedInfoMap.TryGetValue(filterName, out extendedInfo);
        }
    }
}
