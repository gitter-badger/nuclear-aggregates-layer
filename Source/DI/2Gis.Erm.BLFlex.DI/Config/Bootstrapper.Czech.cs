using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureCzechSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container
                        .RegisterType<ICheckInnService, CzechTicService>(Lifetime.Singleton)
                        .RegisterType<IEvaluateBargainNumberService, EvaluateBargainNumberService>(Lifetime.Singleton, new InjectionConstructor("S_{0}-{1}-{2}"));
        }
    }
}
