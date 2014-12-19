﻿using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
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
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
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
                .RegisterType<IBillsConsistencyService, BillsConsistencyService>(Lifetime.PerResolve,
                                                                           new InjectionConstructor(new ResolvedArrayParameter<IBillConsistencyRule>(typeof(ChileBillNumberFormatConsistencyRule),
                                                                                                                                               typeof(BillSummConsistencyRule),
                                                                                                                                               typeof(BillDublicateNumbersConsistencyRule),
                                                                                                                                               typeof(BillDatesConsistencyRule))))
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
        internal static void ConfigureChileListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.Rut);
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListBankDto>(
                x => x.Name);
            FilteredFieldsMetadata.RegisterFilteredFields<ChileListCommuneDto>(
                x => x.Name);

            RelationalMetadata.RegisterRelatedFilter<ChileListLegalPersonDto>(EntityName.Client, x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ChileListBankDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ChileListCommuneDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);

            ExtendedInfoMetadata.RegisterExtendedInfo("DListBanks", "ActiveAndNotDeleted=true");
            ExtendedInfoMetadata.RegisterExtendedInfo("DListCommunes", "ActiveAndNotDeleted=true");

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ChileListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ChileListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<ChileListLegalPersonDto, bool>("ForMe", value =>
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
