using System;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureCzechSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<IFormatterFactory, CzechFormatterFactory>(Lifetime.Singleton)
                        .RegisterType<ICheckInnService, CzechTicService>(Lifetime.Singleton)
                        .RegisterType<ILegalPersonProfileConsistencyRuleContainer, CzechLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("S_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}"))
                        .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton);
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureCzechListingMetadata()
        {
            FilteredFieldMetadata.RegisterFilteredFields<CzechListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.Ic,
                x => x.Dic);
            FilteredFieldMetadata.RegisterFilteredFields<CzechListOrderDto>(
                x => x.OrderNumber,
                x => x.FirmName,
                x => x.ClientName,
                x => x.DestOrganizationUnitName,
                x => x.SourceOrganizationUnitName,
                x => x.BargainNumber,
                x => x.LegalPersonName);

            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonProfileDto>("DListLegalPersonProfiles", x => x.IsActive && !x.IsDeleted);

            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersons", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsInactive", x => !x.IsActive && !x.IsDeleted);
            // Мои юридические лица
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListMyLegalPersons", x => x.IsActive && !x.IsDeleted);
            // Мои юридические лица с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListMyLegalPersonsWithDebt", x => x.IsActive && !x.IsDeleted);
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsWithMyOrders", x => x.IsActive && !x.IsDeleted);
            // Все юридические лица по филиалу
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsAtMyBranch", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица по филиалу с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CzechListLegalPersonDto>("DListLegalPersonsWithDebtAtMyBranch", x => x.IsActive && !x.IsDeleted);

            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListActiveOrders", x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now));
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListInactiveOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Archive);
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListRejectedOrders", x => !x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListAllOrders", x => !x.IsDeleted);
            // Все мои активные заказы
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyActiveOrders", x => x.IsActive && !x.IsDeleted);
            // Мои заказы на расторжении
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyOrdersOnTermination", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnTermination);
            // Мои заказы в статусе На утверждении
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyOrdersOnApproval", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Мои неактивные (заказы закрытые отказом)
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyTerminatedOrders", x => !x.IsDeleted && x.IsTerminated);
            // Мои заказы, у которых отсутствуют подписанные документы
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyOrdersWithDocumentsDebt", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent);
            // Все заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Неактивные (закрытые отказом) заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListTerminatedOrdersForSubordinates", x => !x.IsDeleted && x.IsTerminated);
            // Все мои заказы с типом Самореклама
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMySelfAdsOrders", x => x.IsActive && !x.IsDeleted && x.OrderTypeEnum == OrderType.SelfAds);
            // Все мои заказы с типом Бартер
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyBarterOrders", x => x.IsActive && !x.IsDeleted && (x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter));
            // Мои новые заказы
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyNewOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Новые заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListNewOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие продления
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersToProlongate", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Заказы моих подчиненных, требующие продления
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersToProlongateForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Отклоненные заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListRejectedOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected);
            // Мои заказы в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListMyOrdersToNextEdition", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Заказы моих подчиненных в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersToNextEditionForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Все отклоненные мною БЗ
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListRejectedByMeOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие моего одобрения
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersOnApprovalForMe", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListApprovedOrdersWithoutAdvertisement", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Заказы в выпуск следующего месяца закрытые отказом
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListTerminatedOrdersForNextMonthEdition", x => !x.IsDeleted && x.IsTerminated);
            // Неподписанные БЗ за текущий выпуск
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListOrdersWithDocumentsDebtForNextMonth", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Список технических расторжений
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical);
            // Список действительных расторжений
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListNonTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None);
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            DefaultFilterMetadata.RegisterFilter<CzechListOrderDto>("DListRejectedByMeOrdersOnRegistration", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnRegistration);

            RelationalMetadata.RegisterRelatedFilter<CzechListLegalPersonProfileDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListLegalPersonDto>(EntityName.Client, parentId => x => x.ClientId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListOrderDto>(EntityName.Account, parentId => x => x.AccountId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListOrderDto>(EntityName.Client, parentId => x => x.ClientId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListOrderDto>(EntityName.Deal, parentId => x => x.DealId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListOrderDto>(EntityName.Firm, parentId => x => x.FirmId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CzechListOrderDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId);
        }
    }
}
