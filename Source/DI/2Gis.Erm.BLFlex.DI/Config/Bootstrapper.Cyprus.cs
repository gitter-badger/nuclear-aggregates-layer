using System;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
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
        internal static IUnityContainer ConfigureCyprusSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<IFormatterFactory, FormatterFactory>(Lifetime.Singleton)
                        .RegisterType<ICheckInnService, CyprusTicService>(Lifetime.Singleton)
                        .RegisterType<ILegalPersonProfileConsistencyRuleContainer, CyprusLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-bill"))
                        .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton);
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureCyprusListingMetadata()
        {
            FilteredFieldMetadata.RegisterFilteredFields<CyprusListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.ShortName,
                x => x.LegalAddress,
                x => x.Tic,
                x => x.Vat,
                x => x.PassportNumber);
            FilteredFieldMetadata.RegisterFilteredFields<CyprusListOrderDto>(
                x => x.OrderNumber,
                x => x.FirmName,
                x => x.ClientName,
                x => x.DestOrganizationUnitName,
                x => x.SourceOrganizationUnitName,
                x => x.BargainNumber,
                x => x.LegalPersonName);

            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersons", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsInactive", x => !x.IsActive && !x.IsDeleted);
            // Мои юридические лица
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListMyLegalPersons", x => x.IsActive && !x.IsDeleted);
            // Мои юридические лица с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListMyLegalPersonsWithDebt", x => x.IsActive && !x.IsDeleted);
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsWithMyOrders", x => x.IsActive && !x.IsDeleted);
            // Все юридические лица по филиалу
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsAtMyBranch", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица по филиалу с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<CyprusListLegalPersonDto>("DListLegalPersonsWithDebtAtMyBranch", x => x.IsActive && !x.IsDeleted);

            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListActiveOrders", x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now));
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListInactiveOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Archive);
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListRejectedOrders", x => !x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListAllOrders", x => !x.IsDeleted);
            // Все мои активные заказы
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyActiveOrders", x => x.IsActive && !x.IsDeleted);
            // Мои заказы на расторжении
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyOrdersOnTermination", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnTermination);
            // Мои заказы в статусе На утверждении
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyOrdersOnApproval", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Мои неактивные (заказы закрытые отказом)
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyTerminatedOrders", x => !x.IsDeleted && x.IsTerminated);
            // Мои заказы, у которых отсутствуют подписанные документы
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyOrdersWithDocumentsDebt", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent);
            // Все заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Неактивные (закрытые отказом) заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListTerminatedOrdersForSubordinates", x => !x.IsDeleted && x.IsTerminated);
            // Все мои заказы с типом Самореклама
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMySelfAdsOrders", x => x.IsActive && !x.IsDeleted && x.OrderTypeEnum == OrderType.SelfAds);
            // Все мои заказы с типом Бартер
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyBarterOrders", x => x.IsActive && !x.IsDeleted && (x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter));
            // Мои новые заказы
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyNewOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Новые заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListNewOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие продления
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersToProlongate", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Заказы моих подчиненных, требующие продления
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersToProlongateForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Отклоненные заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListRejectedOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected);
            // Мои заказы в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListMyOrdersToNextEdition", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Заказы моих подчиненных в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersToNextEditionForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Все отклоненные мною БЗ
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListRejectedByMeOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие моего одобрения
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersOnApprovalForMe", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListApprovedOrdersWithoutAdvertisement", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Заказы в выпуск следующего месяца закрытые отказом
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListTerminatedOrdersForNextMonthEdition", x => !x.IsDeleted && x.IsTerminated);
            // Неподписанные БЗ за текущий выпуск
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListOrdersWithDocumentsDebtForNextMonth", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Список технических расторжений
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical);
            // Список действительных расторжений
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListNonTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None);
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            DefaultFilterMetadata.RegisterFilter<CyprusListOrderDto>("DListRejectedByMeOrdersOnRegistration", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnRegistration);

            DefaultFilterMetadata.RegisterFilter<ListFirmDto>("DListReservedFirmsLefkosia", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment && (x.LastDisqualifyTime == null || ((DateTime.Now.Month - x.LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - x.LastDisqualifyTime.Value.Year)) > 2) && x.OrganizationUnitId == 133);
            DefaultFilterMetadata.RegisterFilter<ListFirmDto>("DListReservedFirmsLemesos", x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment && (x.LastDisqualifyTime == null || ((DateTime.Now.Month - x.LastDisqualifyTime.Value.Month) + 12 * (DateTime.Now.Year - x.LastDisqualifyTime.Value.Year)) > 2) && x.OrganizationUnitId == 122);

            RelationalMetadata.RegisterRelatedFilter<CyprusListLegalPersonDto>(EntityName.Client, parentId => x => x.ClientId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CyprusListOrderDto>(EntityName.Account, parentId => x => x.AccountId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CyprusListOrderDto>(EntityName.Client, parentId => x => x.ClientId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CyprusListOrderDto>(EntityName.Deal, parentId => x => x.DealId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CyprusListOrderDto>(EntityName.Firm, parentId => x => x.FirmId == parentId);
            RelationalMetadata.RegisterRelatedFilter<CyprusListOrderDto>(EntityName.LegalPerson, parentId => x => x.LegalPersonId == parentId);
        }
    }
}
