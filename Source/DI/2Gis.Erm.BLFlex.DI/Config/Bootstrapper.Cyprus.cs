﻿using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        internal static IUnityContainer ConfigureCyprusSpecific(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            return container.RegisterType<ICheckInnService, CyprusTicService>(Lifetime.Singleton);
        }
    }
}
