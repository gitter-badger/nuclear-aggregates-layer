using System;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Net.Mime;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    // handle mvc-pipeline exceptions and rewrite them as 500 or 403 errors
    public sealed class ExceptionFilter : IExceptionFilter
    {
        private readonly ICommonLog _logger;

        public ExceptionFilter(ICommonLog logger)
        {
            _logger = logger;
        }

        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;

            var httpContext = filterContext.HttpContext;
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;
            var isAjaxRequest = httpRequest.IsAjaxRequest();
            var isDebug = httpContext.IsDebuggingEnabled;

            var errorTitle = HandleException(exception, httpResponse);

            _logger.Fatal(exception, BLResources.CriticalError);

            // ajax request
            if (isAjaxRequest)
            {
                filterContext.Result = new ContentResult { ContentType = MediaTypeNames.Text.Plain, Content = errorTitle };

                // required code
                filterContext.ExceptionHandled = true;
                httpResponse.TrySkipIisCustomErrors = true;

                return;
            }

            // do nothing if custom errors not enabled
            if (!httpContext.IsCustomErrorEnabled)
            {
                return;
            }

            var errorText = isDebug ? exception.ToString() : exception.Message;

            // filterContext.Result
            var viewResult = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<ErrorHandlerModel>(),
                TempData = filterContext.Controller.TempData,
            };

            // Копируем содержимое исходной viewdata, т.к. там может быть необходимая информация:
            // настройки локали и т.п.
            // В конструктор ViewDataDictionary<ErrorHandlerModel> передавать исходную viewdata нельзя,
            // т.к. тип модели скорее всего отличается от ErrorHandlerModel - получим InvalidOperationException
            foreach (var item in filterContext.Controller.ViewData)
            {
                viewResult.ViewData.Add(item);
            }

            viewResult.ViewData.Model = new ErrorHandlerModel
            {
                Title = errorTitle,
                Text = errorText,
            };

            filterContext.Result = viewResult;

            // required code
            filterContext.ExceptionHandled = true;
            httpResponse.TrySkipIisCustomErrors = true;
        }

        // handling erros with code 500 or 403
        public static string HandleException(Exception exception, HttpResponseBase httpResponse)
        {
            var exceptionType = exception.GetType();

            // if-return
            if (exceptionType == typeof(SecurityAccessDeniedException))
            {
                httpResponse.StatusCode = 403;
                return BLResources.AccessDenied;
            }

            // if-else-if
            httpResponse.StatusCode = 500;
            var errorText = BLResources.ApplicationError;

            if (exceptionType == typeof(NotificationException))
            {
                errorText = !string.IsNullOrWhiteSpace(exception.Message) ? exception.Message : BLResources.ErrorDuringOperation;
            }
            else if (typeof(BusinessLogicException).IsAssignableFrom(exceptionType))
            {
                errorText = !string.IsNullOrWhiteSpace(exception.Message) ? exception.Message : BLResources.ApplicationError;
            }
            else if (exceptionType == typeof(CommunicationException))
            {
                errorText = BLResources.CommunicationError;
            }
            else if (exceptionType == typeof(OptimisticConcurrencyException))
            {
                errorText = BLResources.DataHasBeenChanged;
            }
            else if (typeof(DataException).IsAssignableFrom(exceptionType))
            {
                // Таблица с описанием ошибок SqlServer: dbo.sysmessages
                var innerException = exception.InnerException as SqlException;
                if (innerException != null)
                {
                    var errorNumber = innerException.Errors[0].Number;
                    if (errorNumber == 3960)
                    {   // Транзакция в режиме изоляции моментального снимка прервана из-за конфликта обновлений. 
                        // Невозможно использовать режим изоляции моментального снимка для прямого или косвенного 
                        // доступа к таблице "%1!" в базе данных "%2!" для обновления, удаления или вставки 
                        errorText = BLResources.DataHasBeenChanged;
                    }
                    else if (errorNumber == 547)
                    {   // Конфликт инструкции %1! с ограничением %2! "%3!". 
                        // Конфликт произошел в базе данных "%4!", таблица "%5!"%6!%7!%8!.
                        errorText = BLResources.InconsistentData;
                    }
                }
            }

            return errorText;
        }
    }
}