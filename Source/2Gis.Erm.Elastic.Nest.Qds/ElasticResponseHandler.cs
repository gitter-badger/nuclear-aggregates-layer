using System;
using System.Linq;

using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public sealed class ElasticResponseHandler : IElasticResponseHandler
    {
        public void ThrowWhenError(IResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (!response.IsValid)
                throw new ElasticException(response.ConnectionStatus.ToString(), response.ConnectionStatus.OriginalException);

            var bulkResponse = response as IBulkResponse;
            if (bulkResponse != null && bulkResponse.Errors)
            {
                var errorItem = bulkResponse.Items.First(x => !string.IsNullOrEmpty(x.Error));
                throw new ElasticException(string.Format("{0} - {1}", errorItem.Operation, errorItem.Error));
            }
        }
    }
}