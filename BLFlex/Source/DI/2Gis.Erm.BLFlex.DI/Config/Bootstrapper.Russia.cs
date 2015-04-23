using System;

using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Clients;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureRussiaSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                    .RegisterType<IDynamicEntityPropertiesConverter<RussiaLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<RussiaLegalPersonProfilePart>>(Lifetime.Singleton)

                    .RegisterType<IFormatterFactory, RussiaFormatterFactory>(Lifetime.Singleton)
                    .RegisterType<ICheckInnService, RussiaCheckInnService>(Lifetime.Singleton)
                    .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                    .RegisterType<IPartableEntityValidator<BranchOffice>, NullBranchOfficeValidator>(Lifetime.Singleton)
                    .RegisterType<ILegalPersonProfileConsistencyRuleContainer, RussiaLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                    .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IContactSalutationsProvider, RussiaContactSalutationsProvider>(Lifetime.Singleton)
                    .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                           new InjectionConstructor(new ResolvedArrayParameter<IBillConsistencyRule>(typeof(LockedOrderConsistencyRule),
                                                                                                                                               typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDatesConsistencyRule),
                                                                                                                                               typeof(BillDistributionPeriodConsistencyRule))))
                    .RegisterType<IBargainPrintFormDataExtractor, BargainPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IPriceCostsForSubPositionsProvider, MoDiPriceCostsForSubPositionsProvider>(Lifetime.Singleton)
                    .ConfigureRussiaSpecificNumberServices();
        }

        public static IUnityContainer ConfigureRussiaSpecificNumberServices(this IUnityContainer container)
        {
            return container
                    .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("Д_{0}-{1}-{2}", "АД_{0}-{1}-{2}"))
                    .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-счёт"))
                    .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberService>(Lifetime.Singleton, new InjectionConstructor("БЗ_{0}-{1}-{2}", "ОФ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForRussia))
                    .RegisterType<IEvaluateBillDateService, EvaluateBillDateService>();
        }

        internal static void ConfigureRussiaListingMetadata(this IUnityContainer container)
        {
            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListLegalPersonDto, bool>("ForMe", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                if (value)
                {
                    return x => x.OwnerCode == userId;
                }

                return x => x.OwnerCode != userId;
            });

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("ActiveBusinessMeaning", value => x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now));
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("NotDeleted", value => x => !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("Approved", value => x => x.WorkflowStepEnum == OrderState.Approved);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("Archive", value => x => x.WorkflowStepEnum == OrderState.Archive);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("OnTermination", value => x => x.WorkflowStepEnum == OrderState.OnTermination);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("OnApproval", value => x => x.WorkflowStepEnum == OrderState.OnApproval);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("OnRegistration", value => x => x.WorkflowStepEnum == OrderState.OnRegistration);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("Rejected", value => x => x.WorkflowStepEnum == OrderState.Rejected);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("AllActiveStatuses", value => x => x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("Absent", value => x => x.HasDocumentsDebtEnum == DocumentsDebt.Absent);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("SelfAds", value => x => x.OrderTypeEnum == OrderType.SelfAds);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("Barter", value => x => x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("New", value => x => ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("TechnicalTerminated", value => x => (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("NonTechnicalTerminated", value => x => (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("ForNextEdition", value =>
            {
                if (!value)
                {
                    return null;
                }

                var nextMonth = DateTime.Now.AddMonths(1);
                nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                var currentMonthLastDate = nextMonth.AddSeconds(-1);
                var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                return
                    x => x.EndDistributionDateFact >= currentMonthLastDate && x.BeginDistributionDate <= currentMonthFirstDate;
            });
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("ForNextMonthEdition", value =>
            {
                if (!value)
                {
                    return null;
                }

                var tmpMonth = DateTime.Now.AddMonths(2);
                tmpMonth = new DateTime(tmpMonth.Year, tmpMonth.Month, 1);

                var nextMonthLastDate = tmpMonth.AddSeconds(-1);
                var nextMonthFirstDate = new DateTime(nextMonthLastDate.Year, nextMonthLastDate.Month, 1);

                return x => x.EndDistributionDateFact >= nextMonthLastDate && x.BeginDistributionDate <= nextMonthFirstDate;
            });
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("useCurrentMonthForEndDistributionDateFact", value =>
            {
                if (!value)
                {
                    return null;
                }

                var nextMonth = DateTime.Now.AddMonths(1);
                nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                var currentMonth = nextMonth.AddSeconds(-1);

                return x => x.EndDistributionDateFact == currentMonth;
            });
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("ForMe", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListOrderDto, bool>("MyInspection", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.InspectorCode == userId;
            });
        }
    }
}
