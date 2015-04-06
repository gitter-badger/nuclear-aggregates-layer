using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List;
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
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
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
                .RegisterType<IPriceCostsForSubPositionsProvider, NullPriceCostsForSubPositionsProvider>(Lifetime.Singleton)
                .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                           new InjectionConstructor(new ResolvedArrayParameter<IBillConsistencyRule>(typeof(LockedOrderConsistencyRule),
                                                                                                                                               typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDatesConsistencyRule),
                                                                                                                                               typeof(BillDistributionPeriodConsistencyRule))))
                .ConfigureEmiratesSpecificNumberServices();
        }

        public static IUnityContainer ConfigureEmiratesSpecificNumberServices(this IUnityContainer container)
        {
            return container
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}", "AC_{0}-{1}-{2}"))
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}"))
                        .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("Q_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForCountriesWithRomanAlphabet))
                        .RegisterType<IEvaluateBillDateService, EvaluateBillDateService>();
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureEmiratesListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<EmiratesListLegalPersonDto>(
                 x => x.LegalName,
                 x => x.ClientName,
                 x => x.LegalAddress,
                 x => x.CommercialLicense);
            FilteredFieldsMetadata.RegisterFilteredFields<EmiratesListBranchOfficeDto>(
                x => x.Name,
                x => x.CommercialLicense,
                x => x.LegalAddress);
            FilteredFieldsMetadata.RegisterFilteredFields<EmiratesListAcceptanceReportsJournalRecordDto>(
                x => x.OrganizationUnitName);

            RelationalMetadata.RegisterRelatedFilter<EmiratesListLegalPersonDto>(EntityName.Client, x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListAcceptanceReportsJournalRecordDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            ExtendedInfoMetadata.RegisterExtendedInfo("AcceptanceReportsJournal", "ActiveAndNotDeleted=true");

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListBranchOfficeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListBranchOfficeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<EmiratesListLegalPersonDto, bool>("ForMe", value =>
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
