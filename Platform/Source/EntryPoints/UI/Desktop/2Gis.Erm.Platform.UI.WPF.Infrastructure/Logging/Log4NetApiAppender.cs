using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using log4net.Appender;
using log4net.Core;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Logging
{
    /// <summary>
    /// Summary description for ServiceAppender.
    /// </summary>
    public class Log4NetApiAppender : AppenderSkeleton
    {
        private IApiClient _apiClient;

        public Log4NetApiAppender(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        #region Override implementation of AppenderSkeleton

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                LogUsingApi(loggingEvent);
            }
            catch (Exception exc)
            {
                ErrorHandler.Error("Unable to send loggingEvent to Logging Service", exc);
            }
        }

        private async void LogUsingApi(LoggingEvent loggingEvent)
        {
            //_serviceLogging.RMSServiceLogger(
                //    loggingEvent.Level.Name,
                //    loggingEvent.RenderedMessage,
                //    Environment.UserName,
                //    Environment.UserDomainName,
                //    ((AppBaseLoader)AppLoader.Instance).PersonSystemUserGuid,
                //    SystemInfo.HostName);

            var request = new ApiRequest("Error/LogError");
            request.AddParameter(string.Empty, loggingEvent.RenderedMessage);
            var task = _apiClient.PostAsync(request);
            var response = await task;
        }

        protected override bool RequiresLayout
        {
            get
            {
                return false;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            _apiClient = null;
        }

        #endregion
    }
}
