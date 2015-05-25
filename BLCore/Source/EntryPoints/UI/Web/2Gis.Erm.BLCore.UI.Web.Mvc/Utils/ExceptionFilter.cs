using System;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Net.Mime;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    // handle mvc-pipeline exceptions and rewrite them as 500 or 403 errors
    public sealed class ExceptionFilter : IExceptionFilter
    {
        private readonly ITracer _tracer;

        public ExceptionFilter(ITracer tracer)
        {
            _tracer = tracer;
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

            _tracer.Fatal(exception, BLResources.CriticalError);

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

            // �������� ���������� �������� viewdata, �.�. ��� ����� ���� ����������� ����������:
            // ��������� ������ � �.�.
            // � ����������� ViewDataDictionary<ErrorHandlerModel> ���������� �������� viewdata ������,
            // �.�. ��� ������ ������ ����� ���������� �� ErrorHandlerModel - ������� InvalidOperationException
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
            var originalException = exception is TargetInvocationException && exception.InnerException != null ? exception.InnerException : exception;
            var originalExceptionType = originalException.GetType();

            // if-return
            if (originalExceptionType == typeof(SecurityAccessDeniedException))
            {
                httpResponse.StatusCode = 403;
                return BLResources.AccessDenied;
            }

            // if-else-if
            httpResponse.StatusCode = 500;
            var errorText = BLResources.ApplicationError;

            if (originalExceptionType == typeof(NotificationException))
            {
                errorText = !string.IsNullOrWhiteSpace(originalException.Message) ? originalException.Message : BLResources.ErrorDuringOperation;
            }
            else if (typeof(BusinessLogicException).IsAssignableFrom(originalExceptionType))
            {
                errorText = !string.IsNullOrWhiteSpace(originalException.Message) ? originalException.Message : BLResources.ApplicationError;
            }
            else if (originalExceptionType == typeof(CommunicationException))
            {
                errorText = BLResources.CommunicationError;
            }
            else if (originalExceptionType == typeof(OptimisticConcurrencyException))
            {
                errorText = BLResources.DataHasBeenChanged;
            }
            else if (typeof(DataException).IsAssignableFrom(originalExceptionType))
            {
                // ������� � ��������� ������ SqlServer: dbo.sysmessages
                var innerException = originalException.InnerException as SqlException;
                if (innerException != null)
                {
                    var errorNumber = innerException.Errors[0].Number;
                    if (errorNumber == 3960)
                    {
                        // ���������� � ������ �������� ������������� ������ �������� ��-�� ��������� ����������. 
                        // ���������� ������������ ����� �������� ������������� ������ ��� ������� ��� ���������� 
                        // ������� � ������� "%1!" � ���� ������ "%2!" ��� ����������, �������� ��� ������� 
                        errorText = BLResources.DataHasBeenChanged;
                    }
                    else if (errorNumber == 547)
                    {
                        // �������� ���������� %1! � ������������ %2! "%3!". 
                        // �������� ��������� � ���� ������ "%4!", ������� "%5!"%6!%7!%8!.
                        errorText = BLResources.InconsistentData;
                    }
                }
            }

            return errorText;
        }
    }
}