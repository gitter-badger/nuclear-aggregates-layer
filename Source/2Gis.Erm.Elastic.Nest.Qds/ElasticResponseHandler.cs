using System;

using DoubleGis.Erm.Qds;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticResponseHandler : IElasticResponseHandler
    {
        public void ThrowWhenError(IResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (response.IsValid)
                return;

            var result = response.ConnectionStatus.Result;

            var message = !string.IsNullOrEmpty(result) ? result : response.ConnectionStatus.Error.ExceptionMessage;

            throw new ElasticException(message, response.ConnectionStatus.Error.OriginalException);
        }
    }
}