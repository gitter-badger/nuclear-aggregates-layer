using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Authorization;
using DoubleGis.Erm.Qds.API.Operations.Documents;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Extensions;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class ClientIndexer :
        IEntityIndexer<Client>,
        IEntityIndexerIndirect<Client>,
        IDocumentIndexer<ClientGridDoc>
    {
        private readonly IFinder _finder;
        private readonly IUserProfile _userProfile;
        private readonly IElasticApi _elasticApi;
        private readonly ISearchSettings _searchSettings;

        public ClientIndexer(IFinder finder, IUserProfile userProfile, IElasticApi elasticApi, ISearchSettings searchSettings)
        {
            _finder = finder;
            _userProfile = userProfile;
            _elasticApi = elasticApi;
            _searchSettings = searchSettings;
        }

        void IEntityIndexer<Client>.IndexEntities(params long[] ids)
        {
            var clients = _finder.Find<Client>(x => ids.Contains(x.Id));
            var result = GetClientGridDoc(clients);
            _elasticApi.Bulk(result);
        }

        void IEntityIndexerIndirect<Client>.IndexEntitiesIndirectly(IQueryable<Client> query)
        {
            var result = GetClientGridDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<ClientGridDoc>.IndexAllDocuments()
        {
            var clients = _finder.FindAll<Client>();
            var result = GetClientGridDoc(clients, true);
            _elasticApi.BulkExclusive<ClientGridDoc>(result);
        }

        private IEnumerable<BulkDescriptor> GetClientGridDoc(IQueryable<Client> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
            {
                x.Id, 
                x.ReplicationCode, 
                x.Name, 
                x.MainAddress, 
                x.TerritoryId, 
                TerritoryName = x.Territory.Name, 
                x.OwnerCode, 
                x.IsActive, 
                x.IsDeleted, 
                x.CreatedOn, 
                InformationSource = (InformationSource)x.InformationSource, 
                x.Timestamp, 
            });

            // TODO: optimize
            var allUsers = _finder.FindAll<User>().ToArray();

            var indexDescriptors = dtos
                .AsEnumerable()
                .Join(
                allUsers, 
                x => x.OwnerCode, 
                x => x.Id, 
                (x, y) => new BulkIndexDescriptor<ClientGridDoc>()
                    .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                    .Version(x.Timestamp, indirectly)
                    .Object(new ClientGridDoc
                    {
                        ReplicationCode = x.ReplicationCode.ToString(), 
                        Name = x.Name, 
                        MainAddress = x.MainAddress, 
                        TerritoryId = x.TerritoryId, 
                        TerritoryName = x.TerritoryName, 
                        OwnerCode = x.OwnerCode, 
                        OwnerName = y.DisplayName, 
                        IsActive = x.IsActive, 
                        IsDeleted = x.IsDeleted, 
                        CreatedOn = x.CreatedOn, 
                        InformationSource = x.InformationSource.ToStringLocalized(EnumResources.ResourceManager, _userProfile.UserLocaleInfo.UserCultureInfo), 

                        Authorization = new DocumentAuthorization
                        {
                            Tags = new[]
                            {
                                "user/" + x.OwnerCode, 
                                "department/" + y.DepartmentId
                            }, 
                        }, 
                    }));

            return indexDescriptors.Batch(_searchSettings.BatchSize).Select(batch =>
            {
                var bulkDescriptor = new BulkDescriptor();
                foreach (var iterator in batch)
                {
                    var indexDescriptor = iterator;
                    bulkDescriptor.Index<ClientGridDoc>(x => indexDescriptor);
                }

                return bulkDescriptor;
            });
        }
    }
}