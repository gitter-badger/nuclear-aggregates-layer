using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class RussiaAreaRegistration : AreaRegistration
    {
        private const string ControllersNamespace = "DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Russia.Controllers";

        public override string AreaName
        {
            get { return ControllersNamespace; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(ControllersNamespace,
                             "Russia/{controller}/{action}/{id}",
                             new { id = UrlParameter.Optional },
                             new[] { ControllersNamespace });
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class CyprusAreaRegistration : AreaRegistration
    {
        private const string ControllersNamespace = "DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Cyprus.Controllers";

        public override string AreaName
        {
            get { return ControllersNamespace; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(ControllersNamespace,
                             "Cyprus/{controller}/{action}/{id}",
                             new { id = UrlParameter.Optional },
                             new[] { ControllersNamespace });
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class CzechAreaRegistration : AreaRegistration
    {
        private const string ControllersNamespace = "DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Czech.Controllers";

        public override string AreaName
        {
            get { return ControllersNamespace; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(ControllersNamespace,
                             "Czech/{controller}/{action}/{id}",
                             new { id = UrlParameter.Optional },
                             new[] { ControllersNamespace });
        }
    }
}