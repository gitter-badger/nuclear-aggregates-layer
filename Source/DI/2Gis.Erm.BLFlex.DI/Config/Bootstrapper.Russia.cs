using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureRussiaSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                    .RegisterType<IFormatterFactory, RussiaFormatterFactory>(Lifetime.Singleton)
                    .RegisterType<ICheckInnService, RussiaCheckInnService>(Lifetime.Singleton)
                    .RegisterType<IPartableEntityValidator<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitValidator>(Lifetime.Singleton)
                    .RegisterType<IPartableEntityValidator<BranchOffice>, NullBranchOfficeValidator>(Lifetime.Singleton)
                    .RegisterType<ILegalPersonProfileConsistencyRuleContainer, RussiaLegalPersonProfileConsistencyRuleContainer>(Lifetime.Singleton)
                    .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton)
                    .RegisterType<IBusinessModelEntityObtainerFlex<LegalPerson>, NullLegalPersonObtainerFlex>(Lifetime.Singleton)
                    .RegisterType<IBusinessModelEntityObtainerFlex<LegalPersonProfile>, NullLegalPersonProfileObtainerFlex>(Lifetime.Singleton)
                    .RegisterType<IBusinessModelEntityObtainerFlex<BranchOffice>, NullBranchOfficeObtainerFlex>(Lifetime.Singleton)
                    .RegisterType<IBusinessModelEntityObtainerFlex<BranchOfficeOrganizationUnit>, NullBranchOfficeOrganizationUnitObtainerFlex>(Lifetime.Singleton)
                    .ConfigureRussiaSpecificNumberServices();
        }

        public static IUnityContainer ConfigureRussiaSpecificNumberServices(this IUnityContainer container)
        {
            return container
                    .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("Д_{0}-{1}-{2}"))
                    .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-счёт"))
                    .RegisterType<IOrderPrintFormDataExtractor, OrderPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IBargainPrintFormDataExtractor, BargainPrintFormDataExtractor>(Lifetime.PerResolve)
                    .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton)
                    .RegisterType<IEvaluateOrderNumberService, EvaluateOrderNumberService>(Lifetime.Singleton, new InjectionConstructor("БЗ_{0}-{1}-{2}", "ОФ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForRussia));
        }
    }
}
