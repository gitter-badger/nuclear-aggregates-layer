using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Controllers;
using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Controllers
{
    public class EvaluateBillViewsService : IEvaluateBillViewsService
    {
        private readonly BusinessModel _model;
        public EvaluateBillViewsService(BusinessModel model)
        {
            _model = model;
        }

        public string GetCreateView()
        {
            return string.Format("{0}/Create", _model);
        }
    }
}
