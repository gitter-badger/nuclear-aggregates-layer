using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

using DoubleGis.Erm.BL.UI.Web.Mvc.App_Start;

using RazorGenerator.Mvc;

using WebActivatorEx;

[assembly: PostApplicationStartMethod(typeof(RazorGeneratorMvcStart), "Start")]

namespace DoubleGis.Erm.BL.UI.Web.Mvc.App_Start {
    public static class RazorGeneratorMvcStart {
        public static void Start() {
            var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly) {
                UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
            };

            ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}