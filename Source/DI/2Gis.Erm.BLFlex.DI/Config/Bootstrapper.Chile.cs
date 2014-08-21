﻿using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureChileSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<IDynamicEntityPropertiesConverter<ChileLegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<ChileLegalPersonPart>>(Lifetime.Singleton)
                        .RegisterType<IDynamicEntityPropertiesConverter<ChileLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<ChileLegalPersonProfilePart>>(Lifetime.Singleton)
                        .RegisterType<IDynamicEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart>>(Lifetime.Singleton)
                        .RegisterType<IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>, DictionaryEntityEntityPropertiesConverter<Bank>>(Lifetime.Singleton)
                        .RegisterType<IDynamicEntityPropertiesConverter<Commune, DictionaryEntityInstance, DictionaryEntityPropertyInstance>, DictionaryEntityEntityPropertiesConverter<Commune>>(Lifetime.Singleton)

                        .RegisterType<IFormatterFactory, ChileFormatterFactory>(Lifetime.Singleton)
                        .RegisterType<ICheckInnService, ChileRutService>(Lifetime.Singleton)
                        .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, ChileBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                        .RegisterType<IPartableEntityValidator<BranchOffice>, ChileBranchOfficeValidator>(Lifetime.Singleton)
                        .RegisterType<ILegalPersonProfileConsistencyRuleContainer, ChileLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                        .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IValidateBillsService, ChileValidateBillsService>(Lifetime.PerResolve)

                        .ConfigureChileSpecificNumberServices();
        }

        public static IUnityContainer ConfigureChileSpecificNumberServices(this IUnityContainer container)
        {
            return container
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}", "AC_{0}-{1}-{2}")) // http://confluence.2gis.local:8090/pages/viewpage.action?pageId=117179880
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{0}"))
                        .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("ORD_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForCountriesWithRomanAlphabet));
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureChileListingMetadata()
        {
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.Rut);
            FilteredFieldsMetadata.RegisterFilteredFields<MultiCultureListOrderDto>(
                x => x.OrderNumber,
                x => x.FirmName,
                x => x.ClientName,
                x => x.DestOrganizationUnitName,
                x => x.SourceOrganizationUnitName,
                x => x.LegalPersonName);
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListBankDto>(
                x => x.Name);
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListCommuneDto>(
                x => x.Name);

            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersons", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsInactive", x => !x.IsActive && !x.IsDeleted);
            // Мои юридические лица
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListMyLegalPersons", x => x.IsActive && !x.IsDeleted);
            // Мои юридические лица с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListMyLegalPersonsWithDebt", x => x.IsActive && !x.IsDeleted);
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsWithMyOrders", x => x.IsActive && !x.IsDeleted);
            // Все юридические лица по филиалу
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsAtMyBranch", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица по филиалу с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<ChileListLegalPersonDto>("DListLegalPersonsWithDebtAtMyBranch", x => x.IsActive && !x.IsDeleted);

            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListActiveOrders", x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now));
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListInactiveOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Archive);
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListRejectedOrders", x => !x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListAllOrders", x => !x.IsDeleted);
            // Все мои активные заказы
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyActiveOrders", x => x.IsActive && !x.IsDeleted);
            // Мои заказы на расторжении
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyOrdersOnTermination", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnTermination);
            // Мои заказы в статусе На утверждении
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyOrdersOnApproval", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Мои неактивные (заказы закрытые отказом)
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyTerminatedOrders", x => !x.IsDeleted && x.IsTerminated);
            // Мои заказы, у которых отсутствуют подписанные документы
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyOrdersWithDocumentsDebt", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent);
            // Все заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Неактивные (закрытые отказом) заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListTerminatedOrdersForSubordinates", x => !x.IsDeleted && x.IsTerminated);
            // Все мои заказы с типом Самореклама
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMySelfAdsOrders", x => x.IsActive && !x.IsDeleted && x.OrderTypeEnum == OrderType.SelfAds);
            // Все мои заказы с типом Бартер
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyBarterOrders", x => x.IsActive && !x.IsDeleted && (x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter));
            // Мои новые заказы
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyNewOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Новые заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListNewOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved) && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие продления
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersToProlongate", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Заказы моих подчиненных, требующие продления
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersToProlongateForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Approved);
            // Отклоненные заказы моих подчиненных
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListRejectedOrdersForSubordinates", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected);
            // Мои заказы в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListMyOrdersToNextEdition", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Заказы моих подчиненных в ближайший выпуск
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersToNextEditionForSubordinates", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Все отклоненные мною БЗ
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListRejectedByMeOrders", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.Rejected && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            // Заказы, требующие моего одобрения
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersOnApprovalForMe", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // БЗ, в статусе Одобрено, у которых отсутствуют прикрепленные РМ
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListApprovedOrdersWithoutAdvertisement", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnApproval);
            // Заказы в выпуск следующего месяца закрытые отказом
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListTerminatedOrdersForNextMonthEdition", x => !x.IsDeleted && x.IsTerminated);
            // Неподписанные БЗ за текущий выпуск
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListOrdersWithDocumentsDebtForNextMonth", x => x.IsActive && !x.IsDeleted && x.HasDocumentsDebtEnum == DocumentsDebt.Absent && (x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved));
            // Список технических расторжений
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical);
            // Список действительных расторжений
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListNonTechnicalTerminatedOrders", x => x.IsActive && !x.IsDeleted && (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None);
            // Все отклоненные мною заказы, которые сейчас в статусе На оформлении
            DefaultFilterMetadata.RegisterFilter<MultiCultureListOrderDto>("DListRejectedByMeOrdersOnRegistration", x => x.IsActive && !x.IsDeleted && x.WorkflowStepEnum == OrderState.OnRegistration);

            DefaultFilterMetadata.RegisterFilter<ChileListBankDto>("DListBanks", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<ChileListCommuneDto>("DListCommunes", x => x.IsActive && !x.IsDeleted);

            RelationalMetadata.RegisterRelatedFilter<ChileListLegalPersonDto>(EntityName.Client, x => x.ClientId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Account, x => x.AccountId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Client, x => x.ClientId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Deal, x => x.DealId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Firm, x => x.FirmId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.LegalPerson, x => x.LegalPersonId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Bargain, x => x.BargainId);
        }
    }
}
