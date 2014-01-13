using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI
{
    public static class CustomLifetime
    {
        public static LifetimeManager PerRequest
        {
            get { return new UnityPerWebRequestLifetimeManager(); }
        }
    }
}