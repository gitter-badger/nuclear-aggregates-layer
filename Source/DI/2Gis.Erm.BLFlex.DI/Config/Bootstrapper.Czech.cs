using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
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
        }
    }
}
