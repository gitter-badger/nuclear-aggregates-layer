using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Russia.Controllers;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.UI.Web.Mvc.DI
{
    internal static partial class BootstrapperMvc
    {
        private static Interception ConfigureRussiaSpecific(Interception interception)
        {
            Func<ResolvedParameter[]> resolvedParametersCreator =
                () => new ResolvedParameter[]
                    {
                        new ResolvedParameter<ICommonLog>(),
                        new ResolvedParameter<IActionLogger>(Mapping.SimplifiedModelConsumerScope),
                        new ResolvedParameter<IDependentEntityProvider>()
                    };

            interception.SetInterceptorFor<LegalPersonController>(new VirtualMethodInterceptor())
                        .SetVirtualInterceptorForMethod<ClientsMergingController, LogControllerCallHandler>(
                            controller => controller.Merge(default(ClientViewModel)),
                            Policy.ClientInterception,
                            Policy.ClientInterception,
                            Lifetime.PerResolve,
                            new InjectionConstructor(resolvedParametersCreator()));

            return interception;
        }
    }
}