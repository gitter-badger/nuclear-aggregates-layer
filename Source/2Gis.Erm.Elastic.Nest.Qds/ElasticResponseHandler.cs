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

            throw new DocsStorageException(response.ConnectionStatus.Error.ExceptionMessage, response.ConnectionStatus.Error.OriginalException);
        }
    }
}