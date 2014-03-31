using System.Collections.Generic;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class ExtendedInfoMetadata
    {
        private static readonly Dictionary<string, string> ExtendedInfoMap = new Dictionary<string, string>
        {
            // Мои лицевые счета с отрицательным балансом
            {"DListMyAccountsWithNegativeBalance", "ForMe=true" },
            // Все лицевые счета по по филиалу
            {"DListAccountsAtMyBranch", "MyBranch=true" },
            // Лицевые счета моих подчиненных с отрицательным балансом
            {"DListAccountsWithNegativeBalanceForSubordinates", "ForSubordinates=true" },
            // Мои лицевые счета с размещающимися БЗ
            {"DListMyAccountsWithHostedOrders", "WithHostedOrders=true;ForMe=true" },
            // Лицевые счета моих подчиненных с размещающимися БЗ
            {"DListAccountsWithHostedOrdersForSubordinates", "WithHostedOrders=true;ForSubordinates=true" },
            // Мои действия
            {"DListMyActivities", "ForMe=true" },
            // Действия по моим подчиненным
            {"DListActivitiesForSubordinates", "ForSubordinates=true" },
            // Мои завершенные действия
            {"DListMyCompletedActivities", "ForMe=true" },
            // Завершенные действия по моим подчиненным
            {"DListCompletedActivitiesForSubordinates", "ForSubordinates=true" },
            // Мои запланированные действия
            {"DListMyActivitiesInProgress", "ForMe=true" },
            // Запланированные действия по моим подчиненным
            {"DListActivitiesInProgressForSubordinates", "ForSubordinates=true" },
            // Мои запланированные действия на сегодня
            {"DListMyActivitiesInProgressForToday", "ForToday=true;ForMe=true" },
            // Действия по теплым клиентам
            {"DListActivitiesForWarmClients", "Expired=false" },
            // Просроченные действия по теплым клиентам
            {"DListOverdueActivitiesForWarmClients", "Expired=true" },
            // Мои договоры
            {"DListMyBargains", "ForMe=true" },
            // Мои клиенты
            {"DListMyClients", "ForMe=true" },
            // Мои клиенты, созданные сегодня
            {"DListMyClientsCreatedToday", "ForToday=true;ForMe=true" },
            // Клиенты на моей территории
            {"DListClientsOnMyTerritory", "MyTerritory=true" },
            // Мои клиенты с дебиторской задолженностью
            {"DListMyClientsWithDebt", "ForMe=true;WithDebt=true" },
            // Клиенты в резерве на моей территории
            {"DListReservedClientsOnMyTerritory", "MyTerritory=true;ForReserve=true" },
            // Мои теплые клиенты
            {"DListMyWarmClients", "ForMe=true" },
            // Клиенты, у которых есть заказы с типом Бартер
            {"DListClientsWithBarter", "WithBarterOrders=true" },
            // Мои клиенты без ЛПР
            {"DListMyClientsWithoutMakeDecisionContacts", "ForMe=true;NoMakingDecisions=true" },
            // Клиенты по моим подчиненным
            {"DListClientsForSubordinates", "ForSubordinates=true" },
            // Теплые клиенты моих подчиненных
            {"DListWarmClientsForSubordinates", "ForSubordinates=true" },
            // Клиенты моих подчиненных, у которых есть заказы с типом Бартер
            {"DListClientsWithBarterForSubordinates", "ForSubordinates=true;WithBarterOrders=true" },
            // Клиенты моих подчиненных без ЛПР
            {"DListClientsWithoutMakeDecisionContactsForSubordinates", "ForSubordinates=true;NoMakingDecisions=true" },
            // Региональные клиенты моих подчиненных
            {"DListRegionalClientsForSubordinates", "ForSubordinates=true;IsRegional=true" },
            // Клиенты моих подчиненных, у которых несколько открытых сделок
            {"DListClientsWithSeveralOpenDealsForSubordinates", "ForSubordinates=true;MinDealCount=1" },
            // Все клиенты по филиалу
            {"DListClientsAtMyBranch", "MyBranch=true" },
            // Клиенты моих подчиненных с дебиторской задолженностью
            {"DListClientsWithDebtForSubordinates", "ForSubordinates=true;WithDebt=true" },
            // Клиенты, с которыми была только одна встреча
            {"DListClientsWithOnly1Appointment", "With1Appointment=true" },
            // Тёплые клиенты
            {"DListClientsWithWarmClientTask", "WarmClientTask=true;Outdated=false" },
            // Тёплые клиенты с просроченной задачей
            {"DListClientsWithOutdatedWarmClientTask", "WarmClientTask=true;Outdated=true" },
            // Мои контактные лица
            {"DListMyContacts", "ForMe=true" },
            // Мои сделки
            {"DListMyDeals", "ForMe=true" },
            // Мои закрытые сделки
            {"DListMyInactiveDeals", "ForMe=true" },
            // Мои бартерные сделки
            {"DListMyBarterDeals", "ForMe=true;WithBarterOrders=true" },
            // Все сделки по филиалу
            {"DListDealsAtBranch", "MyBranch=true" },
            // Сделки моих подчиненных
            {"DListDealsForSubordinates", "ForSubordinates=true" },
            // Мои фирмы
            {"DListMyFirms", "ForMe=true" },
            // Фирмы в резерве на моей территории
            {"DListReservedFirmsOnMyTerritories", "MyTerritory=true;ForReserve=true" },
            // Новые фирмы моей территории
            {"DListNewFirmsOnMyTerritories", "CreatedInCurrentMonth=true;MyTerritory=true" },
            // Все фирмы по филиалу
            {"DListFirmsAtMyBranch", "MyBranch=true" },
            // Все активные фирмы по филиалу
            {"DListActiveFirmsAtMyBranch", "MyBranch=true" },
            // Фирмы моих подчиненных
            {"DListFirmsForSubordinates", "ForSubordinates=true" },
            // Фирмы с заказами с типом Самореклама
            {"DListFirmsWithSelfAds", "WithSelfAdsOrders=true" },
            // Мои юридические лица
            {"DListMyLegalPersons", "ForMe=true" },
            // Мои юридические лица с дебиторской задолженностью
            {"DListMyLegalPersonsWithDebt", "ForMe=true;WithDebt=true" },
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            {"DListLegalPersonsWithMyOrders", "HasMyOrders=true;ForMe=false" },
            // Все юридические лица по филиалу
            {"DListLegalPersonsAtMyBranch", "MyBranch=true" },
            // Юридические лица моих подчиненных
            {"DListLegalPersonsForSubordinates", "ForSubordinates=true" },
            // Юридические лица моих подчиненных с дебиторской задолженностью
            {"DListLegalPersonsWithDebtForSubordinates", "ForSubordinates=true;WithDebt=true" },
            // Юридические лица по филиалу с дебиторской задолженностью
            {"DListLegalPersonsWithDebtAtMyBranch", "MyBranch=true;WithDebt=true" },
            // Мои открытые лимиты
            {"DListMyOpenedLimits", "useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Мои отклоненные лимиты
            {"DListMyRejectedLimits", "useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Мои одобренные лимиты
            {"DListMyApprovedLimits", "useNextMonthForStartPeriodDate=true;ForMe=true" },
            // Лимиты по моим подчиненным
            {"DListLimitsForSubordinates", "useNextMonthForStartPeriodDate=true;ForSubordinates=true" },
            // Лимиты, требующие моего одобрения
            {"DListLimitsForApprove", "useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Одобренные мною лимиты
            {"DListApprovedByMeLimits", "useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Отклоненные мною лимиты
            {"DListRejectedByMeLimits", "useNextMonthForStartPeriodDate=true;MyInspection=true" },
            // Лимиты по филиалу
            {"DListLimitsAtMyBranch", "MyBranch=true" },
            // Все мои активные заказы
            {"DListMyActiveOrders", "ForMe=true" },
            // Мои заказы на расторжении
            {"DListMyOrdersOnTermination", "ForMe=true" },
            // Мои заказы в статусе На утверждении
            {"DListMyOrdersOnApproval", "ForMe=true" },
            // Мои неактивные (заказы закрытые отказом)
            {"DListMyTerminatedOrders", "ForMe=true" },
            // Мои заказы, у которых отсутствуют подписанные документы
            {"DListMyOrdersWithDocumentsDebt", "ForMe=true" },
            // Все заказы моих подчиненных
            {"DListOrdersForSubordinates", "ForSubordinates=true" },
            // Неактивные (закрытые отказом) заказы моих подчиненных
            {"DListTerminatedOrdersForSubordinates", "ForSubordinates=true" },
            // Все мои заказы с типом Самореклама
            {"DListMySelfAdsOrders", "ForMe=true" },
            // Все мои заказы с типом Бартер
            {"DListMyBarterOrders", "ForMe=true" },
            // Мои новые заказы
            {"DListMyNewOrders", "ForMe=true" },
            // Новые заказы моих подчиненных
            {"DListNewOrdersForSubordinates", "ForSubordinates=true" },
            // Заказы, требующие продления
            {"DListOrdersToProlongate", "useCurrentMonthForEndDistributionDateFact=true" },
            // Заказы моих подчиненных, требующие продления
            {"DListOrdersToProlongateForSubordinates", "ForSubordinates=true;useCurrentMonthForEndDistributionDateFact=true" },
            // Отклоненные заказы моих подчиненных
            {"DListRejectedOrdersForSubordinates", "ForSubordinates=true" },
            // Мои заказы в ближайший выпуск
            {"DListMyOrdersToNextEdition", "ForNextEdition=true;ForMe=true" },
            // Заказы моих подчиненных в ближайший выпуск
            {"DListOrdersToNextEditionForSubordinates", "ForSubordinates=true;ForNextEdition=true" },
            // Все отклоненные мною БЗ
            {"DListRejectedByMeOrders", "MyInspection=true" },
            // Заказы, требующие моего одобрения
            {"DListOrdersOnApprovalForMe", "MyInspection=true" },
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            {"DListApprovedOrdersWithoutAdvertisement", "WithoutAdvertisement=true" },
            // Заказы в выпуск следующего месяца закрытые отказом
            {"DListTerminatedOrdersForNextMonthEdition", "ForNextMonthEdition=true" },
            // Неподписанные БЗ за текущий выпуск
            {"DListOrdersWithDocumentsDebtForNextMonth", "ForNextEdition=true" },
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            {"DListRejectedByMeOrdersOnRegistration", "RejectedByMe=true;MyInspection=true" },
            {"DListReservedFirmsLefkosia", "ForReserve=true" },
            {"DListReservedFirmsLemesos", "ForReserve=true" },
        };

        public static bool TryGetExtendedInfo(string filterName, out string extendedInfo)
        {
            return ExtendedInfoMap.TryGetValue(filterName, out extendedInfo);
        }
    }
}
