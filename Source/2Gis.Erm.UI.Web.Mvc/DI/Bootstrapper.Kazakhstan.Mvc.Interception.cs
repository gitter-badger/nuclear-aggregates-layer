using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Ukraine.Controllers;

using Microsoft.Practices.Unity.InterceptionExtension;
 
namespace DoubleGis.Erm.UI.Web.Mvc.DI
{
    internal static partial class BootstrapperMvc
    {
        private static Interception ConfigureKazakhstanSpecific(Interception interception)
        {
            interception.SetInterceptorFor<LegalPersonController>(new VirtualMethodInterceptor());

            return interception;
        }
    }
}