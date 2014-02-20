using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureRussiaSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                    .RegisterType<ICheckInnService, RussiaCheckInnService>(Lifetime.Singleton)
                    .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("Д_{0}-{1}-{2}"))
                    .RegisterType<IEvaluateBillNumberService, EvaluateBillNumberService>(Lifetime.Singleton, new InjectionConstructor("{1}-счёт"))
                    .RegisterType<IValidateBillsService, NullValidateBillsService>(Lifetime.Singleton);
        }
    }
}
