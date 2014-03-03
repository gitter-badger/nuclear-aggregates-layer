using System.Web.Mvc;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Report
{
    internal sealed class DelayedReportModel : ReportModel
    {
        private readonly IModelBinder _binder;
        private readonly ControllerContext _controllerContext;
        private readonly ModelBindingContext _bindingContext;

        public DelayedReportModel(IModelBinder binder, ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            _binder = binder;
            _controllerContext = controllerContext;
            _bindingContext = bindingContext;
        }

        public ReportModel GetReportModel()
        {
            var model = _binder.BindModel(_controllerContext, _bindingContext);
            return model is DelayedReportModel ? null : (ReportModel)model;
        }
    }
}