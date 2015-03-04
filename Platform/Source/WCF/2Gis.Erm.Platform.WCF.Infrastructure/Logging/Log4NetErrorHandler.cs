using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class Log4NetErrorHandler : IErrorHandler
    {
        private readonly ITracer _logger;

        public Log4NetErrorHandler(ITracer logger)
        {
            _logger = logger;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            var faultException = error as FaultException;
            if (faultException != null)
            {
                var fault = faultException.CreateMessageFault();
                if (fault.HasDetail)
                {
                    var details = fault.GetReaderAtDetailContents().ReadContentAsString();
                    _logger.WarnFormat("FaultException was thrown in ERM WCF service. Details: {0}",
                        !string.IsNullOrEmpty(details) ? details : faultException.ToString());
                }
                else
                {
                    _logger.WarnFormat("FaultException was thrown in ERM WCF service. Details: {0}", faultException.ToString());
                }
            }
            else
            {
                _logger.Fatal(error, "Unhandled exception has occured in ERM WCF service");
            }

            return false;
        }
    }
}