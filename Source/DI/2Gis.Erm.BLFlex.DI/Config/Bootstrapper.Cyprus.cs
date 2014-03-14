using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;

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
        }
    }
}
