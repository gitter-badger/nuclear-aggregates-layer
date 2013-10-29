using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace DoubleGis.Erm.SqlIdentityClient
{
    public static class SqlFunctions
    {
        [SqlFunction(DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None, IsDeterministic = false, TableDefinition = "Id bigint")]
        public static IEnumerable GetIdentifiers(string service, int count)
        {
            const int MaxIdentifiersAtOnce = 32767;

            var binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.MaxReceivedMessageSize = MaxIdentifiersAtOnce * 100 + 1024;
            var serviceClient = new IdentityProvider.IdentityProviderApplicationServiceClient(binding, new EndpointAddress(service));

            var result = new List<long>(count);
            while (count > 0)
            {
                var batchSize = count > MaxIdentifiersAtOnce ? MaxIdentifiersAtOnce : count;
                var generatedIds = serviceClient.GetIdentities(batchSize);
                result.AddRange(generatedIds);
                count -= batchSize;
            }

            return result;
        }
    }
}
