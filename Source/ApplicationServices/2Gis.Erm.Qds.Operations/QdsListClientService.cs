using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Common.Extensions;
using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Qds.Operations
{
    public sealed class QdsListClientService : ListEntityDtoServiceBase<Platform.Model.Entities.Erm.Client, ClientGridDoc>
    {
        private readonly IUserContext _userContext;
        private readonly IElasticClientFactory _elasticClientFactory;

        public QdsListClientService(IUserContext userContext, IElasticClientFactory elasticClientFactory)
        {
            _userContext = userContext;
            _elasticClientFactory = elasticClientFactory;
        }

        protected override IEnumerable<ClientGridDoc> List(QuerySettings querySettings, out int count)
        {
            var queryResponse = _elasticClientFactory.UsingElasticClient(elasticClient =>
            {
                // TODO: вынести в базовый класс или в сервис
                var user = elasticClient.Get<UserDoc>(_userContext.Identity.Code.ToString());
                if (user == null)
                {
                    throw new SecurityAccessDeniedException("Cannot find user");
                }

                return elasticClient
                    .Search<ClientGridDoc>(x => x
                    .Query(y => y.ApplyQuerySettings(querySettings))
                    .Filter(y => y.ApplyUserPermissions(user))
                    .ApplySortingPaging(querySettings));
            });

            count = queryResponse.Total;
            return queryResponse.Documents;
        }
    }
}