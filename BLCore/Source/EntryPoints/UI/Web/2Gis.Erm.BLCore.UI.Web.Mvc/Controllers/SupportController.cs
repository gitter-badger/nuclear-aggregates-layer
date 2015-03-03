using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class SupportController : Controller
    {
        private readonly ISupportSettings _supportSettings;

        public SupportController(ISupportSettings supportSettings)
        {
            _supportSettings = supportSettings;
        }

        public ActionResult Index()
        {
            return Redirect(_supportSettings.SupportUrl);
        }
    }
}