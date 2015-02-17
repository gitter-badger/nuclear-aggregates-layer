using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
{
    public static class ModelUtils
    {
        #region model handling utils

        public static bool CheckIsModelValid(Controller controller, ViewModel model)
        {
            if (controller.ModelState.IsValid)
            {
                return true;
            }

            // create human-readable error string and set it to model message
            var errors = new StringBuilder();
            foreach (var error in controller.ModelState.Values.SelectMany(modelState => modelState.Errors).Select(error => error.ErrorMessage).Distinct())
            {
                errors.AppendLine(error);
            }

            model.SetCriticalError(errors.ToString());
            return false;
        }

        public static void OnException(Controller controller, ICommonLog commonLog, ViewModel model, Exception exception)
        {
            commonLog.Error(exception, BLResources.ErrorDuringOperation);
            var errorText = ExceptionFilter.HandleException(exception, controller.Response);
            model.SetCriticalError(errorText);
        }

        #endregion
    }
}