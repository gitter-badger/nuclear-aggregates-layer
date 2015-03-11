using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Kazakhstan.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Kazakhstan.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureKazakhstanSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<KazakhstanLegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<KazakhstanLegalPersonPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<KazakhstanLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<KazakhstanLegalPersonProfilePart>>(Lifetime.Singleton)
                
                .RegisterType<ILegalPersonProfileConsistencyRuleContainer, KazakhstanLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                .RegisterType<IFormatterFactory, KazakhstanFormatterFactory>(Lifetime.Singleton)
                .RegisterType<ICheckInnService, KazakhstanBinInnService>(Lifetime.Singleton)
                .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IPartableEntityValidator<BranchOffice>, NullBranchOfficeValidator>(Lifetime.PerResolve, Mapping.Erm)
                .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                           new InjectionConstructor(new ResolvedArrayParameter<IBillConsistencyRule>(typeof(LockedOrderConsistencyRule),
                                                                                                                                               typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDatesConsistencyRule),
                                                                                                                                               typeof(BillDistributionPeriodConsistencyRule))))
                .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                .RegisterType<IPriceCostsForSubPositionsProvider, NullPriceCostsForSubPositionsProvider>(Lifetime.Singleton)
                .ConfigureKazakhstanSpecificNumberServices();
        }

        internal static IUnityContainer ConfigureKazakhstanSpecificNumberServices(this IUnityContainer container)
        {
            return container
                .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("Д_{0}-{1}-{2}", "АД_{0}-{1}-{2}"))
                .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}"))
                .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberService>(Lifetime.Singleton, new InjectionConstructor("БЗ_{0}-{1}-{2}", "БЗ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForRussia))
                .RegisterType<IEvaluateBillDateService, EvaluateBillDateService>();
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureKazakhstanListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<KazakhstanListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.BinIin);
            FilteredFieldsMetadata.RegisterFilteredFields<KazakhstanListBranchOfficeDto>(
                x => x.Name,
                x => x.BinIin,
                x => x.LegalAddress);

            RelationalMetadata.RegisterRelatedFilter<KazakhstanListLegalPersonDto>(EntityType.Instance.Client(), x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<KazakhstanListBranchOfficeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<KazakhstanListBranchOfficeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<KazakhstanListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<KazakhstanListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<KazakhstanListLegalPersonDto, bool>("ForMe", value =>
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
