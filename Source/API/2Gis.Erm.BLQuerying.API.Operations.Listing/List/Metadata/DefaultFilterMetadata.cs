using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class DefaultFilterMetadata
    {
        private static readonly Dictionary<Tuple<Type, string>, Expression> FilterMap = new Dictionary<Tuple<Type, string>, Expression>()

            .RegisterFilter<ListAccountDto>("DListAccounts", x => x.IsActive && !x.IsDeleted)
            // Мои лицевые счета с отрицательным балансом
            .RegisterFilter<ListAccountDto>("DListMyAccountsWithNegativeBalance", x => x.IsActive && !x.IsDeleted && x.Balance < 0)
            // Все лицевые счета по по филиалу
            .RegisterFilter<ListAccountDto>("DListAccountsAtMyBranch", x => x.IsActive && !x.IsDeleted)
            // Лицевые счета моих подчиненных с отрицательным балансом
            .RegisterFilter<ListAccountDto>("DListAccountsWithNegativeBalanceForSubordinates", x => x.IsActive && !x.IsDeleted && x.Balance < 0)
            // Мои лицевые счета с размещающимися БЗ
            .RegisterFilter<ListAccountDto>("DListMyAccountsWithHostedOrders", x => x.IsActive && !x.IsDeleted)
            // Лицевые счета моих подчиненных с размещающимися БЗ
            .RegisterFilter<ListAccountDto>("DListAccountsWithHostedOrdersForSubordinates", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListAccountDetailDto>("DListAccountDetails", x => !x.IsDeleted)
            .RegisterFilter<ListAccountDetailDto>("DListAccountDetailsWithDeletedOperations", x => x.IsDeleted)

            .RegisterFilter<ListActivityDto>("DListAllActivities", x => !x.IsDeleted)
            // Активные действия
            .RegisterFilter<ListActivityDto>("DListActiveActivities", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress)
            // Закрытые действия
            .RegisterFilter<ListActivityDto>("DListInactiveActivities", x => x.IsDeleted || !x.IsActive || x.StatusEnum == ActivityStatus.Completed || x.StatusEnum == ActivityStatus.Canceled)
            // Мои действия
            .RegisterFilter<ListActivityDto>("DListMyActivities", x =>x.IsActive && !x.IsDeleted)
            // Действия по моим подчиненным
            .RegisterFilter<ListActivityDto>("DListActivitiesForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Мои завершенные действия
            .RegisterFilter<ListActivityDto>("DListMyCompletedActivities", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.Completed)
            // Завершенные действия по моим подчиненным
            .RegisterFilter<ListActivityDto>("DListCompletedActivitiesForSubordinates", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.Completed)
            // Мои запланированные действия
            .RegisterFilter<ListActivityDto>("DListMyActivitiesInProgress", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress)
            // Запланированные действия по моим подчиненным
            .RegisterFilter<ListActivityDto>("DListActivitiesInProgressForSubordinates", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress)
            // Мои запланированные действия на сегодня
            .RegisterFilter<ListActivityDto>("DListMyActivitiesInProgressForToday", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress)
            // Действия по теплым клиентам
            .RegisterFilter<ListActivityDto>("DListActivitiesForWarmClients", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress && x.TaskType == TaskType.WarmClient)
            // Просроченные действия по теплым клиентам
            .RegisterFilter<ListActivityDto>("DListOverdueActivitiesForWarmClients", x => x.IsActive && !x.IsDeleted && x.StatusEnum == ActivityStatus.InProgress && x.TaskType == TaskType.WarmClient)

            .RegisterFilter<ListAdditionalFirmServiceDto>("AdditionalFirmServices", x => true)

            .RegisterFilter<ListAdsTemplatesAdsElementTemplateDto>("DListAdsTemplatesAdsElementTemplate", x => !x.IsDeleted)

            .RegisterFilter<ListAdvertisementElementDenialReasonsDto>("DListAdvertisementElementDenialReason", x => true)
            .RegisterFilter<ListAdvertisementElementDenialReasonsDto>("DListAdvertisementElementDenialReasonForEdit", x => x.IsActive || x.Checked)

            .RegisterFilter<ListDenialReasonDto>("DListDenialReason", x => x.IsActive)
            .RegisterFilter<ListDenialReasonDto>("DListInactiveDenialReason", x => !x.IsActive)
            .RegisterFilter<ListAdvertisementTemplateDto>("DListAdvertisementTemplate", x => !x.IsDeleted)
            .RegisterFilter<ListAdvertisementTemplateDto>("DListAdvertisementTemplateDeleted", x => x.IsDeleted)

            .RegisterFilter<ListAdvertisementElementTemplateDto>("DListAdvertisementElementTemplate", x => !x.IsDeleted)
            .RegisterFilter<ListAdvertisementElementTemplateDto>("DListAdvertisementElementTemplateDeleted", x => x.IsDeleted)

            .RegisterFilter<ListAdvertisementDto>("DListActiveAdvertisement", x => !x.IsDeleted)
            .RegisterFilter<ListAdvertisementDto>("DListInactiveAdvertisement", x => x.IsDeleted)

            .RegisterFilter<ListAdvertisementElementDto>("DListAdvertisementElement", x => true)

            .RegisterFilter<ListAssociatedPositionDto>("DListAssociatedPosition", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListAssociatedPositionDto>("DListAssociatedPositionInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListAssociatedPositionsGroupDto>("DListAssociatedPositionsGroup", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListAssociatedPositionsGroupDto>("DListAssociatedPositionsGroupInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBargainDto>("DListBargains", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBargainDto>("DListInactiveBargains", x => !x.IsActive && !x.IsDeleted)
            
            // Мои договоры
            .RegisterFilter<ListBargainDto>("DListMyBargains", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBargainFileDto>("DListBargainFiles", x => !x.IsDeleted)

            .RegisterFilter<ListBargainTypeDto>("DListActiveBargainTypes", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListBargainTypeDto>("DListInactiveBargainTypes", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBillDto>("DListBill", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBranchOfficeDto>("DListBranchOfficeActive", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListBranchOfficeDto>("DListBranchOfficeInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListBranchOfficeOrganizationUnitDto>("DListBranchOfficeOrganizationUnit", x => x.IsActive && !x.IsDeleted && !x.OrganizationUnitIsDeleted && !x.BranchOfficeIsDeleted)
            .RegisterFilter<ListBranchOfficeOrganizationUnitDto>("DListBranchOfficeOrganizationUnitInactive", x => !x.IsActive && !x.IsDeleted && !x.OrganizationUnitIsDeleted && !x.BranchOfficeIsDeleted)

            .RegisterFilter<ListCategoryDto>("DListActiveCategories", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListCategoryDto>("DListInactiveCategories", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListCategoryFirmAddressDto>("DListActiveCategoryFirmAddresses", x => x.IsActive && !x.IsDeleted && x.CategoryIsActive && !x.CategoryIsDeleted && x.CategoryOrganizationUnitIsActive && !x.CategoryOrganizationUnitIsDeleted)
            .RegisterFilter<ListCategoryFirmAddressDto>("DListInactiveCategoryFirmAddresses", x => !x.IsActive && !x.IsDeleted || !x.CategoryIsActive && !x.CategoryIsDeleted || !x.CategoryOrganizationUnitIsActive && !x.CategoryOrganizationUnitIsDeleted)

            .RegisterFilter<ListCategoryGroupDto>("DListActiveCategoryGroups", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListCategoryGroupDto>("DListAllCategoryGroups", x => !x.IsDeleted)

            .RegisterFilter<ListCategoryOrganizationUnitDto>("DListCategoryOrganizationUnits", x => !x.IsDeleted && !x.CategoryIsDeleted)

            .RegisterFilter<ListClientDto>("DListClients", x => x.IsActive && !x.IsDeleted)
            // Мои клиенты
            .RegisterFilter<ListClientDto>("DListMyClients", x => x.IsActive && !x.IsDeleted)
            // Мои клиенты, созданные сегодня
            .RegisterFilter<ListClientDto>("DListMyClientsCreatedToday", x => x.IsActive && !x.IsDeleted)
            // Клиенты на моей территории
            .RegisterFilter<ListClientDto>("DListClientsOnMyTerritory", x => x.IsActive && !x.IsDeleted)
            // Мои клиенты с дебиторской задолженностью
            .RegisterFilter<ListClientDto>("DListMyClientsWithDebt", x => x.IsActive && !x.IsDeleted)
            // Клиенты в резерве на моей территории
            .RegisterFilter<ListClientDto>("DListReservedClientsOnMyTerritory", x => x.IsActive && !x.IsDeleted)
            // Мои теплые клиенты
            .RegisterFilter<ListClientDto>("DListMyWarmClients", x => x.IsActive && !x.IsDeleted && x.InformationSourceEnum == InformationSource.WarmClient)
            // Клиенты, у которых есть заказы с типом Бартер
            .RegisterFilter<ListClientDto>("DListClientsWithBarter", x => x.IsActive && !x.IsDeleted)
            // Мои клиенты без ЛПР
            .RegisterFilter<ListClientDto>("DListMyClientsWithoutMakeDecisionContacts", x => x.IsActive && !x.IsDeleted)
            // Клиенты по моим подчиненным
            .RegisterFilter<ListClientDto>("DListClientsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Теплые клиенты моих подчиненных
            .RegisterFilter<ListClientDto>("DListWarmClientsForSubordinates", x => x.IsActive && !x.IsDeleted && x.InformationSourceEnum == InformationSource.WarmClient)
            // Клиенты моих подчиненных, у которых есть заказы с типом Бартер
            .RegisterFilter<ListClientDto>("DListClientsWithBarterForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Клиенты моих подчиненных без ЛПР
            .RegisterFilter<ListClientDto>("DListClientsWithoutMakeDecisionContactsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Региональные клиенты моих подчиненных
            .RegisterFilter<ListClientDto>("DListRegionalClientsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Клиенты моих подчиненных, у которых несколько открытых сделок
            .RegisterFilter<ListClientDto>("DListClientsWithSeveralOpenDealsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Все клиенты по филиалу
            .RegisterFilter<ListClientDto>("DListClientsAtMyBranch", x => x.IsActive && !x.IsDeleted)
            // Клиенты моих подчиненных с дебиторской задолженностью
            .RegisterFilter<ListClientDto>("DListClientsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Клиенты, с которыми была только одна встреча
            .RegisterFilter<ListClientDto>("DListClientsWithOnly1Appointment", x => x.IsActive && !x.IsDeleted)
            // Тёплые клиенты
            .RegisterFilter<ListClientDto>("DListClientsWithWarmClientTask", x => x.IsActive && !x.IsDeleted)
            // Тёплые клиенты с просроченной задачей
            .RegisterFilter<ListClientDto>("DListClientsWithOutdatedWarmClientTask", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListContactDto>("DListActiveContacts", x => x.IsActive && !x.IsDeleted && !x.IsFired)
            .RegisterFilter<ListContactDto>("DListFiredContacts", x => x.IsActive && !x.IsDeleted && x.IsFired)
            // Мои контактные лица
            .RegisterFilter<ListContactDto>("DListMyContacts", x => x.IsActive && !x.IsDeleted && !x.IsFired)

            .RegisterFilter<ListContributionTypeDto>("DListActiveContributionType", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListContributionTypeDto>("DListInactiveContributionType", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListCountryDto>("DListCountries", x => !x.IsDeleted)

            .RegisterFilter<ListCurrencyDto>("DListCurrencies", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListCurrencyDto>("DListCurrenciesInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListCurrencyRateDto>("DListCurrencyRates", x => true)

            .RegisterFilter<ListDealDto>("DListActiveDeals", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListDealDto>("DListInactiveDeals", x => !x.IsActive && !x.IsDeleted)
            // Мои сделки
            .RegisterFilter<ListDealDto>("DListMyDeals", x => x.IsActive && !x.IsDeleted)
            // Мои закрытые сделки
            .RegisterFilter<ListDealDto>("DListMyInactiveDeals", x => !x.IsActive && !x.IsDeleted)
            // Мои бартерные сделки
            .RegisterFilter<ListDealDto>("DListMyBarterDeals", x => x.IsActive && !x.IsDeleted)
            // Все сделки по филиалу
            .RegisterFilter<ListDealDto>("DListDealsAtBranch", x => x.IsActive && !x.IsDeleted)
            // Сделки моих подчиненных
            .RegisterFilter<ListDealDto>("DListDealsForSubordinates", x => true)

            .RegisterFilter<ListDeniedPositionDto>("DListDeniedPosition", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListDeniedPositionDto>("DListDeniedPositionInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListDepartmentDto>("DListDepartment", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListDepartmentDto>("DListInactiveDepartment", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListFirmDto>("DListActiveFirms", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment)
            .RegisterFilter<ListFirmDto>("DListInactiveFirms", x => x.IsActive && !x.IsDeleted && x.ClosedForAscertainment)
            // Мои фирмы
            .RegisterFilter<ListFirmDto>("DListMyFirms", x => x.IsActive && !x.IsDeleted)
            // Фирмы в резерве на моей территории
            .RegisterFilter<ListFirmDto>("DListReservedFirmsOnMyTerritories", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment && (x.LastDisqualifyTime == null || ((DateTime.Now.Month - x.LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - x.LastDisqualifyTime.Value.Year)) > 2))
            // Новые фирмы моей территории
            .RegisterFilter<ListFirmDto>("DListNewFirmsOnMyTerritories", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment)
            // Все фирмы по филиалу
            .RegisterFilter<ListFirmDto>("DListFirmsAtMyBranch", x => !x.IsDeleted)
            // Все активные фирмы по филиалу
            .RegisterFilter<ListFirmDto>("DListActiveFirmsAtMyBranch", x => x.IsActive && !x.IsDeleted)
            // Фирмы моих подчиненных
            .RegisterFilter<ListFirmDto>("DListFirmsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Фирмы с заказами с типом Самореклама
            .RegisterFilter<ListFirmDto>("DListFirmsWithSelfAds", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListFirmAddressDto>("DListActiveFirmAddresses", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment)
            .RegisterFilter<ListFirmAddressDto>("DListInactiveFirmAddresses", x => !x.IsDeleted && (!x.IsActive || x.ClosedForAscertainment))

            .RegisterFilter<ListFirmContactDto>("DListFirmContacts", x => true)

            .RegisterFilter<ListLegalPersonDto>("DListLegalPersons", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsInactive", x => !x.IsActive && !x.IsDeleted)
            // Мои юридические лица
            .RegisterFilter<ListLegalPersonDto>("DListMyLegalPersons", x => x.IsActive && !x.IsDeleted)
            // Мои юридические лица с дебиторской задолженностью
            .RegisterFilter<ListLegalPersonDto>("DListMyLegalPersonsWithDebt", x => x.IsActive && !x.IsDeleted)
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsWithMyOrders", x => x.IsActive && !x.IsDeleted)
            // Все юридические лица по филиалу
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsAtMyBranch", x => x.IsActive && !x.IsDeleted)
            // Юридические лица моих подчиненных
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Юридические лица моих подчиненных с дебиторской задолженностью
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Юридические лица по филиалу с дебиторской задолженностью
            .RegisterFilter<ListLegalPersonDto>("DListLegalPersonsWithDebtAtMyBranch", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListLimitDto>("DListLimits", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListLimitDto>("DListLimitsInactive", x => !x.IsActive && !x.IsDeleted)

            // Мои открытые лимиты
            .RegisterFilter<ListLimitDto>("DListMyOpenedLimits", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Opened)
            // Мои отклоненные лимиты
            .RegisterFilter<ListLimitDto>("DListMyRejectedLimits", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Rejected)
            // Мои одобренные лимиты
            .RegisterFilter<ListLimitDto>("DListMyApprovedLimits", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Approved)
            // Лимиты по моим подчиненным
            .RegisterFilter<ListLimitDto>("DListLimitsForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Лимиты, требующие моего одобрения
            .RegisterFilter<ListLimitDto>("DListLimitsForApprove", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Opened)
            // Одобренные мною лимиты
            .RegisterFilter<ListLimitDto>("DListApprovedByMeLimits", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Approved)
            // Отклоненные мною лимиты
            .RegisterFilter<ListLimitDto>("DListRejectedByMeLimits", x => x.IsActive && !x.IsDeleted && x.StatusEnum == LimitStatus.Rejected)
            // Лимиты по филиалу
            .RegisterFilter<ListLimitDto>("DListLimitsAtMyBranch", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListLocalMessageDto>("DListLocalMessageAll", x => true)
            .RegisterFilter<ListLocalMessageDto>("DListLocalMessageActive", x => (x.StatusEnum == LocalMessageStatus.NotProcessed || x.StatusEnum == LocalMessageStatus.WaitForProcess || x.StatusEnum == LocalMessageStatus.Processing) && x.ReceiverSystemEnum == IntegrationSystem.Erm)
            .RegisterFilter<ListLocalMessageDto>("DListLocalMessageProcessed", x => x.StatusEnum == LocalMessageStatus.Processed && x.ReceiverSystemEnum == IntegrationSystem.Erm)
            .RegisterFilter<ListLocalMessageDto>("DListLocalMessageFailed", x => x.StatusEnum == LocalMessageStatus.Failed && x.ReceiverSystemEnum == IntegrationSystem.Erm)
            .RegisterFilter<ListLocalMessageDto>("DListLocalMessageOutbox", x => x.SenderSystemEnum == IntegrationSystem.Erm)

            .RegisterFilter<ListLockDto>("DListActiveLocks", x => x.IsActive)
            .RegisterFilter<ListLockDto>("DListNotActiveLocks", x => !x.IsActive)

            .RegisterFilter<ListLegalPersonProfileDto>("DListLegalPersonProfiles", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListLockDetailDto>("DListActiveLockDetails", x => x.IsActive)
            .RegisterFilter<ListLockDetailDto>("DListNotActiveLockDetails", x => !x.IsActive)

            .RegisterFilter<ListOperationDto>("DListOperations", x => true)
            .RegisterFilter<ListOperationDto>("DListOperationsAfterSaleService", x => x.TypeEnum == BusinessOperation.AfterSaleServiceActivitiesCreation)

            .RegisterFilter<ListOrderPositionDto>("DListOrderPositions", x => !x.IsDeleted)

            .RegisterFilter<ListOperationTypeDto>("DListOperationTypes", x => true)

            .RegisterFilter<ListOrderPositionAdvertisementDto>("DListOrderPositionAdvertisements", x => true)

            .RegisterFilter<ListOrderDto>("DListActiveOrders", x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now))
            .RegisterFilter<ListOrderDto>("DListInactiveOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Archive)
            .RegisterFilter<ListOrderDto>("DListRejectedOrders", x => !x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListOrderDto>("DListAllOrders", x => !x.IsDeleted)
            // Все мои активные заказы
            .RegisterFilter<ListOrderDto>("DListMyActiveOrders", x => x.IsActive && !x.IsDeleted)
            // Мои заказы на расторжении
            .RegisterFilter<ListOrderDto>("DListMyOrdersOnTermination", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnTermination)
            // Мои заказы в статусе На утверждении
            .RegisterFilter<ListOrderDto>("DListMyOrdersOnApproval", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval)
            // Мои неактивные (заказы закрытые отказом)
            .RegisterFilter<ListOrderDto>("DListMyTerminatedOrders", x => !x.IsActive && !x.IsDeleted)
            // Мои заказы, у которых отсутствуют подписанные документы
            .RegisterFilter<ListOrderDto>("DListMyOrdersWithDocumentsDebt", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent)
            // Все заказы моих подчиненных
            .RegisterFilter<ListOrderDto>("DListOrdersForSubordinates", x => x.IsActive && !x.IsDeleted)
            // Неактивные (закрытые отказом) заказы моих подчиненных
            .RegisterFilter<ListOrderDto>("DListTerminatedOrdersForSubordinates", x => !x.IsActive && !x.IsDeleted)
            // Все мои заказы с типом Самореклама
            .RegisterFilter<ListOrderDto>("DListMySelfAdsOrders", x => x.IsActive && !x.IsDeleted && x.OrderTypeEnum == OrderType.SelfAds)
            // Все мои заказы с типом Бартер
            .RegisterFilter<ListOrderDto>("DListMyBarterOrders", x => x.IsActive && !x.IsDeleted && (x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter))
            // Мои новые заказы
            .RegisterFilter<ListOrderDto>("DListMyNewOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0)
            // Новые заказы моих подчиненных
            .RegisterFilter<ListOrderDto>("DListNewOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0)
            // Заказы, требующие продления
            .RegisterFilter<ListOrderDto>("DListOrdersToProlongate", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved)
            // Заказы моих подчиненных, требующие продления
            .RegisterFilter<ListOrderDto>("DListOrdersToProlongateForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved)
            // Отклоненные заказы моих подчиненных
            .RegisterFilter<ListOrderDto>("DListRejectedOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected)
            // Мои заказы в ближайший выпуск
            .RegisterFilter<ListOrderDto>("DListMyOrdersToNextEdition", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved))
            // Заказы моих подчиненных в ближайший выпуск
            .RegisterFilter<ListOrderDto>("DListOrdersToNextEditionForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved))
            // Все отклоненные мною БЗ
            .RegisterFilter<ListOrderDto>("DListRejectedByMeOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0)
            // Заказы, требующие моего одобрения
            .RegisterFilter<ListOrderDto>("DListOrdersOnApprovalForMe", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval)
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            .RegisterFilter<ListOrderDto>("DListApprovedOrdersWithoutAdvertisement", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval)
            // Заказы в выпуск следующего месяца закрытые отказом
            .RegisterFilter<ListOrderDto>("DListTerminatedOrdersForNextMonthEdition", x => !x.IsActive && !x.IsDeleted)
            // Неподписанные БЗ за текущий выпуск
            .RegisterFilter<ListOrderDto>("DListOrdersWithDocumentsDebtForNextMonth", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved))
            // Список технических расторжений
            .RegisterFilter<ListOrderDto>("DListTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical)
            // Список действительных расторжений
            .RegisterFilter<ListOrderDto>("DListNonTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None)
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            .RegisterFilter<ListOrderDto>("DListRejectedByMeOrdersOnRegistration", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnRegistration)

            .RegisterFilter<ListOrderProcessingRequestDto>("DListOrderProcessingRequest", x => !x.IsDeleted)

            .RegisterFilter<ListOrderFileDto>("DListOrderFiles", x => !x.IsDeleted)

            .RegisterFilter<ListOrganizationUnitDto>("DListOrganizationUnitActive", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListOrganizationUnitDto>("DListOrganizationUnitInactive", x => !x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListOrganizationUnitDto>("DListOrganizationUnitActiveMovedToErm", x => x.IsActive && !x.IsDeleted && x.ErmLaunchDate != null)
            .RegisterFilter<ListOrganizationUnitDto>("DListOrganizationUnitActiveMovedToIR", x => x.IsActive && !x.IsDeleted && x.InfoRussiaLaunchDate != null)

            .RegisterFilter<ListPlatformDto>("DListPlatform", x => true)

            .RegisterFilter<ListPositionDto>("DListPositions", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListPositionDto>("DListPositionInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListPositionCategoryDto>("DListPositionCategory", x => !x.IsDeleted)

            .RegisterFilter<ListPositionChildrenDto>("DListPositionChildren", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListPricePositionDto>("DListPricePositions", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListPricePositionDto>("DListPricePositionsInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListPriceDto>("DListPrices", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListPriceDto>("DListPricesInactive", x => !x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListPrintFormTemplateDto>("DListPrintFormTemplates", x => !x.IsDeleted)

            .RegisterFilter<ListProjectDto>("DListProjects", x => x.IsActive)

            .RegisterFilter<ListRegionalAdvertisingSharingDto>("DListRegionalAdvertisingSharing", x => true)

            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfo", x => true)
            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfoSuccessed", x => x.StatusEnum == ReleaseStatus.Success)
            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfoInProgress", x => x.StatusEnum == ReleaseStatus.InProgressInternalProcessingStarted || x.StatusEnum == ReleaseStatus.InProgressWaitingExternalProcessing)
            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfoFailed", x => x.StatusEnum == ReleaseStatus.Error)
            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfoBeta", x => x.IsBeta)
            .RegisterFilter<ListReleaseInfoDto>("DListReleaseInfoFinal", x => !x.IsBeta)

            .RegisterFilter<ListRoleDto>("DListRole", x => true)

            .RegisterFilter<ListThemeDto>("DListThemeActive", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListThemeTemplateDto>("DListThemeTemplateActive", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListThemeOrganizationUnitDto>("DListUserOrganizationUnit", x => !x.IsDeleted)

            .RegisterFilter<ListThemeCategoryDto>("DListThemeCategory", x => !x.IsDeleted)

            .RegisterFilter<ListTimeZoneDto>("DListTimeZones", x => true)

            .RegisterFilter<ListTerritoryDto>("DListTerritoryActive", x => x.IsActive)
            .RegisterFilter<ListTerritoryDto>("DListTerritoryInactive", x => !x.IsActive)

            .RegisterFilter<ListUserDto>("DListUser", x => x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListUserDto>("DListInactiveUser", x => !x.IsActive && !x.IsDeleted)
            .RegisterFilter<ListUserDto>("DListUserWithRole", x => x.IsActive && !x.IsDeleted)

            .RegisterFilter<ListUserRoleDto>("DListUserRole", x => true)

            .RegisterFilter<ListUserOrganizationUnitDto>("DListUserOrganizationUnit", x => x.UserIsActive && !x.UserIsDeleted)
            .RegisterFilter<ListUserOrganizationUnitDto>("DListUsersInOrganizationUnit", x => x.UserIsActive && !x.UserIsDeleted)

            .RegisterFilter<ListUserTerritoryDto>("DListUserTerritory", x => !x.IsDeleted)

            .RegisterFilter<ListWithdrawalInfoDto>("DListWithdrawalInfo", x => true)
            ;

        private static Dictionary<Tuple<Type, string>, Expression> RegisterFilter<TDocument>(this Dictionary<Tuple<Type, string>, Expression> map, string filterName, Expression<Func<TDocument, bool>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), filterName);
            map.Add(key, expression);
            return map;
        }

        // может вызываться несколько раз, поэтому есть ContainsKey
        public static void RegisterFilter<TDocument>(string filterName, Expression<Func<TDocument, bool>> expression)
        {
            var key = Tuple.Create(typeof(TDocument), filterName);

            if (!FilterMap.ContainsKey(key))
            {
                FilterMap.RegisterFilter(filterName, expression);
            }
        }

        public static bool TryGetFilter<TDocument>(string filterName, out Expression expression)
        {
            if (string.IsNullOrEmpty(filterName))
            {
                expression = FilterMap.Where(x => x.Key.Item1 == typeof(TDocument)).Select(x => x.Value).First();
                return true;
            }

            var key = Tuple.Create(typeof(TDocument), filterName);
            return FilterMap.TryGetValue(key, out expression);
        }
    }
}
