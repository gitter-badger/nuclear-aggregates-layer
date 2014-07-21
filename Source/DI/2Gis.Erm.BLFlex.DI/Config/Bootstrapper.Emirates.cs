using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
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
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureEmiratesSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<AcceptanceReportsJournalRecord, DictionaryEntityInstance, DictionaryEntityPropertyInstance>,
                    DictionaryEntityEntityPropertiesConverter<AcceptanceReportsJournalRecord>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<EmiratesLegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<EmiratesLegalPersonPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<EmiratesLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<EmiratesLegalPersonProfilePart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<EmiratesClientPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<EmiratesClientPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<EmiratesFirmAddressPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<EmiratesFirmAddressPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<EmiratesBranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<EmiratesBranchOfficeOrganizationUnitPart>>(Lifetime.Singleton)

                .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IPartableEntityValidator<BranchOffice>, EmiratesBranchOfficeValidator>(Lifetime.PerResolve, Mapping.Erm)
                .RegisterTypeWithDependencies<IPhoneNumbersFormatter, PhoneNumbersFormatter>(Lifetime.PerResolve, Mapping.Erm)
                .RegisterTypeWithDependencies<IPaymentMethodFormatter, PaymentMethodFormatter>(Lifetime.PerResolve, Mapping.Erm)
                .RegisterType<ILegalPersonProfileConsistencyRuleContainer, EmiratesLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                .RegisterType<IFormatterFactory, EmiratesFormatterFactory>(Lifetime.Singleton)
                .RegisterType<ICheckInnService, EmiratesInnService>(Lifetime.Singleton)
                .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton)
                .ConfigureEmiratesSpecificNumberServices();
        }

        public static IUnityContainer ConfigureEmiratesSpecificNumberServices(this IUnityContainer container)
        {
            return container
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}", "AC_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}"))
                        .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("Q_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForCountriesWithRomanAlphabet));
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureEmiratesListingMetadata()
        {
            FilteredFieldMetadata.RegisterFilteredFields<EmiratesListLegalPersonDto>(
                 x => x.LegalName,
                 x => x.ClientName,
                 x => x.LegalAddress,
                 x => x.CommercialLicense);
            FilteredFieldMetadata.RegisterFilteredFields<EmiratesListBranchOfficeDto>(
                x => x.Name,
                x => x.CommercialLicense,
                x => x.LegalAddress);
            FilteredFieldMetadata.RegisterFilteredFields<MultiCultureListOrderDto>(
                x => x.OrderNumber,
                x => x.FirmName,
                x => x.ClientName,
                x => x.DestOrganizationUnitName,
                x => x.SourceOrganizationUnitName,
                x => x.LegalPersonName);
            FilteredFieldMetadata.RegisterFilteredFields<EmiratesListAcceptanceReportsJournalRecordDto>(
                x => x.OrganizationUnitName);

            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersons", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsInactive", x => !x.IsActive && !x.IsDeleted);
            // Мои юридические лица
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListMyLegalPersons", x => x.IsActive && !x.IsDeleted);
            // Мои юридические лица с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListMyLegalPersonsWithDebt", x => x.IsActive && !x.IsDeleted);
            // Юр.лица, куратором которого я не являюсь, но у которого есть мои заказы
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsWithMyOrders", x => x.IsActive && !x.IsDeleted);
            // Все юридические лица по филиалу
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsAtMyBranch", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица моих подчиненных с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsWithDebtForSubordinates", x => x.IsActive && !x.IsDeleted);
            // Юридические лица по филиалу с дебиторской задолженностью
            DefaultFilterMetadata.RegisterFilter<EmiratesListLegalPersonDto>("DListLegalPersonsWithDebtAtMyBranch", x => x.IsActive && !x.IsDeleted);

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

            DefaultFilterMetadata.RegisterFilter<EmiratesListAcceptanceReportsJournalRecordDto>("AcceptanceReportsJournal", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<EmiratesListBranchOfficeDto>("DListBranchOfficeActive", x => x.IsActive && !x.IsDeleted);
            DefaultFilterMetadata.RegisterFilter<EmiratesListBranchOfficeDto>("DListBranchOfficeInactive", x => !x.IsActive && !x.IsDeleted);

            RelationalMetadata.RegisterRelatedFilter<EmiratesListLegalPersonDto>(EntityName.Client, x => x.ClientId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Account, x => x.AccountId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Client, x => x.ClientId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Deal, x => x.DealId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Firm, x => x.FirmId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.LegalPerson, x => x.LegalPersonId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Bargain, x => x.BargainId);
        }
    }
}
