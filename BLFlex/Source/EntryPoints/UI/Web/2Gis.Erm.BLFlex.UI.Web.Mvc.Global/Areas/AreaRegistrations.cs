using System;
using System.Linq;
using System.Web.Mvc;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas
{
    public abstract class AreaRegistratorBase : AreaRegistration
    {
        private const string ControllersSuffix = "Controllers";

        private readonly string _controllersNamespace;
        private readonly string _areaName;

        protected AreaRegistratorBase()
        {
            var typeInfo = GetType();
            var targetAreaNamespace = typeInfo.Namespace;
            if (string.IsNullOrEmpty(targetAreaNamespace))
            {
                throw new InvalidOperationException("Unsupported area registrator location. Registrator: " + typeInfo.FullName);
            }

            _controllersNamespace = targetAreaNamespace + "." + ControllersSuffix;
            _areaName = targetAreaNamespace.Split('.').Last();
        }

        public override string AreaName
        {
            get { return _areaName; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(_areaName + "_default",
                             _areaName + "/{controller}/{action}/{id}",
                             new { id = UrlParameter.Optional },
                             new[] { _controllersNamespace });
        }
    }
}