using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Controllers;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Controllers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureChileSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<ICheckInnService, ChileRutService>(Lifetime.Singleton)
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("C_{0}-{1}-{2}")) // http://confluence.2gis.local:8090/pages/viewpage.action?pageId=117179880
                        .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{0}"))
                        .RegisterType<IEvaluateBillViewsService, EvaluateBillViewsService>(Lifetime.Singleton, new InjectionConstructor(BusinessModel.Chile))
                        .RegisterType<IValidateBillsService, ChileValidateBillsService>(Lifetime.PerResolve);
        }
    }
}
