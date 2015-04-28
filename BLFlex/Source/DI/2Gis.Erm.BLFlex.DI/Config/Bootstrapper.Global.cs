using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        private static readonly IReadOnlyDictionary<BusinessModel, Func<IUnityContainer, IGlobalizationSettings, IUnityContainer>>
            ConfiguratorsMap = new Dictionary<BusinessModel, Func<IUnityContainer, IGlobalizationSettings, IUnityContainer>>
                {
                    { BusinessModel.Russia, ConfigureRussiaSpecific },
                    { BusinessModel.Cyprus, ConfigureCyprusSpecific },
                    { BusinessModel.Czech, ConfigureCzechSpecific },
                    { BusinessModel.Chile, ConfigureChileSpecific },
                    { BusinessModel.Ukraine, ConfigureUkraineSpecific },
                    { BusinessModel.Emirates, ConfigureEmiratesSpecific },
                    { BusinessModel.Kazakhstan, ConfigureKazakhstanSpecific },
                };

        private static readonly IReadOnlyDictionary<BusinessModel, Action<IUnityContainer>>
            ConfiguratorsListingMap = new Dictionary<BusinessModel, Action<IUnityContainer>>
                {
                    { BusinessModel.Russia, ConfigureRussiaListingMetadata },
                    { BusinessModel.Cyprus, ConfigureCyprusListingMetadata },
                    { BusinessModel.Czech, ConfigureCzechListingMetadata },
                    { BusinessModel.Chile, ConfigureChileListingMetadata },
                    { BusinessModel.Ukraine, ConfigureUkraineListingMetadata },
                    { BusinessModel.Emirates, ConfigureEmiratesListingMetadata },
                    { BusinessModel.Kazakhstan, ConfigureKazakhstanListingMetadata },
                };

        public static IUnityContainer ConfigureGlobal(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            var businessModelSpecificConfigurator = ConfiguratorsMap[globalizationSettings.BusinessModel];
            return businessModelSpecificConfigurator(container, globalizationSettings);
        }

        // HACK дико извиняюсь, но пока метаданные для листинга регистрируются только так, скоро поправим
        public static void ConfigureGlobalListing(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<MultiCultureListOrderDto>(
                x => x.Number,
                x => x.FirmName,
                x => x.ClientName,
                x => x.DestOrganizationUnitName,
                x => x.SourceOrganizationUnitName,
                x => x.LegalPersonName);

            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Account, x => x.AccountId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Client, x => x.ClientId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Deal, x => x.DealId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Firm, x => x.FirmId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.LegalPerson, x => x.LegalPersonId);
            RelationalMetadata.RegisterRelatedFilter<MultiCultureListOrderDto>(EntityName.Bargain, x => x.BargainId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("ActiveBusinessMeaning", value => x => (x.IsActive && !x.IsDeleted && x.WorkflowStepEnum != OrderState.Archive) || (!x.IsDeleted && (x.WorkflowStepEnum == OrderState.Archive || x.WorkflowStepEnum == OrderState.OnTermination) && x.EndDistributionDateFact > DateTime.Now));
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("NotDeleted", value => x => !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("Approved", value => x => x.WorkflowStepEnum == OrderState.Approved);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("Archive", value => x => x.WorkflowStepEnum == OrderState.Archive);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("OnTermination", value => x => x.WorkflowStepEnum == OrderState.OnTermination);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("OnApproval", value => x => x.WorkflowStepEnum == OrderState.OnApproval);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("OnRegistration", value => x => x.WorkflowStepEnum == OrderState.OnRegistration);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("Rejected", value => x => x.WorkflowStepEnum == OrderState.Rejected);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("AllActiveStatuses", value => x => x.WorkflowStepEnum == OrderState.OnRegistration || x.WorkflowStepEnum == OrderState.OnApproval || x.WorkflowStepEnum == OrderState.Rejected || x.WorkflowStepEnum == OrderState.Approved);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("Absent", value => x => x.HasDocumentsDebtEnum == DocumentsDebt.Absent);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("SelfAds", value => x => x.OrderTypeEnum == OrderType.SelfAds);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("Barter", value => x => x.OrderTypeEnum == OrderType.AdsBarter || x.OrderTypeEnum == OrderType.ProductBarter || x.OrderTypeEnum == OrderType.ServiceBarter);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("New", value => x => ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) <= 2 && ((x.BeginDistributionDate.Month - DateTime.Now.Month) + 12 * (x.BeginDistributionDate.Year - DateTime.Now.Year)) > 0);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("TechnicalTerminated", value => x => (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum == OrderTerminationReason.RejectionTechnical);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("NonTechnicalTerminated", value => x => (x.WorkflowStepEnum == OrderState.OnTermination || x.IsTerminated) && x.TerminationReasonEnum != OrderTerminationReason.RejectionTechnical && x.TerminationReasonEnum != OrderTerminationReason.None);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("ForNextEdition", value =>
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
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("ForNextMonthEdition", value =>
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
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("useCurrentMonthForEndDistributionDateFact", value =>
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
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("ForMe", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<MultiCultureListOrderDto, bool>("MyInspection", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                return x => x.InspectorCode == userId;
            });

            Action<IUnityContainer> action;
            if (ConfiguratorsListingMap.TryGetValue(globalizationSettings.BusinessModel, out action))
            {
                action(container);
            }
        }
    }
}
