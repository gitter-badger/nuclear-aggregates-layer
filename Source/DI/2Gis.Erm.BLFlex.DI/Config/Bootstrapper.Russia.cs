using DoubleGis.Erm.BL.Aggregates.Common.Crosscutting;
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
            return container.RegisterType<ICheckInnService, RussiaCheckInnService>(Lifetime.Singleton);
        }
    }
}
