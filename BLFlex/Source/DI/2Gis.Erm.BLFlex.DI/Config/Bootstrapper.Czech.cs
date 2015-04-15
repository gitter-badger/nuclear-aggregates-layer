using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Clients;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
                        .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                        .RegisterType<IPartableEntityValidator<BranchOffice>, NullBranchOfficeValidator>(Lifetime.Singleton)
                        .RegisterType<ILegalPersonProfileConsistencyRuleContainer, CzechLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                        .RegisterType<IContactSalutationsProvider, CzechContactSalutationsProvider>(Lifetime.Singleton)
                        .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                           new InjectionConstructor(new ResolvedArrayParameter<IBillConsistencyRule>(typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDistributionPeriodConsistencyRule))))
                        .RegisterType<IBargainPrintFormDataExtractor, BargainPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IPriceCostsForSubPositionsProvider, NullPriceCostsForSubPositionsProvider>(Lifetime.Singleton)
                        .ConfigureCzechSpecificNumberServices();
        }

        public static IUnityContainer ConfigureCzechSpecificNumberServices(this IUnityContainer container)
        {
            return container
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("S_{0}-{1}-{2}", "AS_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}"))
                        .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("OBJ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForCountriesWithRomanAlphabet))
                        .RegisterType<IEvaluateBillDateService, CzechEvaluateBillDateService>();
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureCzechListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<CzechListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.Ic,
                x => x.Dic);

            RelationalMetadata.RegisterRelatedFilter<CzechListLegalPersonProfileDto>(EntityName.LegalPerson, x => x.LegalPersonId);
            RelationalMetadata.RegisterRelatedFilter<CzechListLegalPersonDto>(EntityName.Client, x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CzechListLegalPersonProfileDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CzechListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CzechListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CzechListLegalPersonDto, bool>("ForMe", value =>
            {
                var userContext = container.Resolve<IUserContext>();
                var userId = userContext.Identity.Code;
                if (value)
                {
                    return x => x.OwnerCode == userId;
                }

                return x => x.OwnerCode != userId;
            });
        }
    }
}
