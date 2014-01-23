﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Qds.API.Operations.Documents;

using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Common.Extensions;

namespace DoubleGis.Erm.Qds.Operations
{
    public sealed class ListClientService : ListEntityDtoServiceBase<Platform.Model.Entities.Erm.Client, ListClientDto>
    {
        private readonly IUserContext _userContext;
        private readonly IElasticClientFactory _elasticClientFactory;

        public ListClientService(IQuerySettingsProvider querySettingsProvider, IFinderBaseProvider finderBaseProvider, IFinder finder, IUserContext userContext, IElasticClientFactory elasticClientFactory)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userContext = userContext;
            _elasticClientFactory = elasticClientFactory;
        }

        protected override IEnumerable<ListClientDto> GetListData(IQueryable<Platform.Model.Entities.Erm.Client> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var queryResponse = _elasticClientFactory.UsingElasticClient(elasticClient =>
            {
                // TODO: вынести в базовый класс или в сервис
                var user = elasticClient.Search<UserDoc>(x => x.Query(y => y.Term("_id", _userContext.Identity.Code))).Documents.FirstOrDefault();
                if (user == null)
                {
                    throw new SecurityAccessDeniedException("Cannot find user");
                }

                return elasticClient.Search<ClientGridDoc>(x => x
                    .Query(y => y.Bool(z => z.Must(p => p
                        .ApplyQuerySettings(querySettings)
                        .ApplyUserPermissions(user))))
                    .ApplySortingPaging(querySettings));
            });

            count = queryResponse.Total;
            return queryResponse.DocumentsWithMetaData.Select(x => new ListClientDto
            {
                Id = long.Parse(x.Id),
                ReplicationCode = Guid.Parse(x.Source.ReplicationCode),
                Name = x.Source.Name,
                MainAddress = x.Source.MainAddress,
                TerritoryId = x.Source.TerritoryId,
                TerritoryName = x.Source.TerritoryName,
                OwnerCode = x.Source.OwnerCode,
                OwnerName = x.Source.OwnerName,
            });
        }
    }
}