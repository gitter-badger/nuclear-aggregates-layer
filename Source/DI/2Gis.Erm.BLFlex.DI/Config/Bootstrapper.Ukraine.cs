﻿using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.Clients;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureUkraineSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<UkraineLegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<UkraineLegalPersonPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<UkraineLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<UkraineLegalPersonProfilePart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<UkraineBranchOfficePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BusinessEntityPropertiesConverter<UkraineBranchOfficePart>>(Lifetime.Singleton)
                
                .RegisterType<ILegalPersonProfileConsistencyRuleContainer, UkraineLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                .RegisterType<IFormatterFactory, UkraineFormatterFactory>(Lifetime.Singleton)
                .RegisterType<ICheckInnService, UkraineIpnService>(Lifetime.Singleton)
                .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IPartableEntityValidator<BranchOffice>, UkraineBranchOfficeValidator>(Lifetime.PerResolve, Mapping.Erm)
                .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton)
                .RegisterType<IContactSalutationsProvider, UkraineContactSalutationsProvider>(Lifetime.Singleton)
                .RegisterType<IUkraineOrderPrintFormDataExtractor, UkraineOrderPrintFormDataExtractor>(Lifetime.PerResolve)
                .ConfigureUkraineSpecificNumberServices();
        }

        public static IUnityContainer ConfigureUkraineSpecificNumberServices(this IUnityContainer container)
        {
            return container
                .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("Д_{0}-{1}-{2}", "АД_{0}-{1}-{2}"))
                .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-счёт"))
                .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberWithoutRegionalService>(Lifetime.Singleton, new InjectionConstructor("БЗ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForRussia));
        }

        // TODO переделать на нормальную метадату
        internal static void ConfigureUkraineListingMetadata(this IUnityContainer container)
        {
            FilteredFieldsMetadata.RegisterFilteredFields<UkraineListLegalPersonDto>(
                x => x.LegalName,
                x => x.ClientName,
                x => x.LegalAddress,
                x => x.Ipn,
                x => x.Egrpou);
            FilteredFieldsMetadata.RegisterFilteredFields<UkraineListBranchOfficeDto>(
                x => x.Name,
                x => x.Ipn,
                x => x.Egrpou,
                x => x.LegalAddress);

            RelationalMetadata.RegisterRelatedFilter<UkraineListLegalPersonDto>(EntityName.Client, x => x.ClientId);

            var extendedInfoFilterMetadata = container.Resolve<IExtendedInfoFilterMetadata>();

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<UkraineListBranchOfficeDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<UkraineListBranchOfficeDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);

            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<UkraineListLegalPersonDto, bool>("ActiveAndNotDeleted", value => x => x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<UkraineListLegalPersonDto, bool>("NotActiveAndNotDeleted", value => x => !x.IsActive && !x.IsDeleted);
            extendedInfoFilterMetadata.RegisterExtendedInfoFilter<UkraineListLegalPersonDto, bool>("ForMe", value =>
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
