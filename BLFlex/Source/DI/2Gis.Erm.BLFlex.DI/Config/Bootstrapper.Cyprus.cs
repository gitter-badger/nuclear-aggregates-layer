using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Clients;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureCyprusSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<IFormatterFactory, CyprusFormatterFactory>(Lifetime.Singleton)
                        .RegisterType<ICheckInnService, CyprusTicService>(Lifetime.Singleton)
                        .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                        .RegisterType<IPartableEntityValidator<BranchOffice>, NullBranchOfficeValidator>(Lifetime.Singleton)
                        .RegisterType<ILegalPersonProfileConsistencyRuleContainer, CyprusLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                .RegisterType<IContactSalutationsProvider, CyprusContactSalutationsProvider>(Lifetime.Singleton)
                        .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                                 new InjectionConstructor(
                                                                                     new ResolvedArrayParameter<IBillConsistencyRule>(typeof(LockedOrderConsistencyRule),
                                                                                                                                               typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDatesConsistencyRule),
                                                                                                                                               typeof(BillDistributionPeriodConsistencyRule))))
                        .RegisterType<IBargainPrintFormDataExtractor, BargainPrintFormDataExtractor>(Lifetime.PerResolve)
                        .RegisterType<IPriceCostsForSubPositionsProvider, NullPriceCostsForSubPositionsProvider>(Lifetime.Singleton)
                        .ConfigureCyprusSpecificNumberServices();
        }

        public static IUnityContainer ConfigureCyprusSpecificNumberServices(this IUnityContainer container)
        {
            return container
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}", "AC_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-bill"))
                        .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("INV_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForCountriesWithRomanAlphabet))
                        .RegisterType<IEvaluateBillDateService, EvaluateBillDateService>();
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureCyprusListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<CyprusListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.ShortName,
                x => x.LegalAddress,
                x => x.Tic,
                x => x.Vat,
                x => x.PassportNumber);

            RelationalMetadata.RegisterRelatedFilter<CyprusListLegalPersonDto>(EntityName.Client, x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListFirmDto, bool>("Lefkosia", value => x => x.OrganizationUnitId == 133);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ListFirmDto, bool>("Lemesos", value => x => x.OrganizationUnitId == 122);

            ExtendedInfoMetadata.RegisterExtendedInfo("DListReservedFirmsLefkosia", "ForReserve=true;Lefkosia=true");
            ExtendedInfoMetadata.RegisterExtendedInfo("DListReservedFirmsLemesos", "ForReserve=true;Lemesos=true");

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CyprusListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CyprusListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<CyprusListLegalPersonDto, bool>("ForMe", value =>
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
