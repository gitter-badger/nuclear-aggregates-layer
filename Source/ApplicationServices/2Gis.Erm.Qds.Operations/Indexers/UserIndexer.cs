using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Extensions;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class UserIndexer :
        IEntityIndexer<User>,
        IEntityIndexerIndirect<User>,
        IDocumentIndexer<UserDoc>
    {
        // TODO: пока вычитываем только операции чтеня клиентов
        private const int ClientEntityTypeId = 200;
        private const int ReadOperationId = 1;

        private readonly IFinder _finder;
        private readonly IEntityIndexerIndirect<Client> _clientIndexer;
        private readonly IElasticApi _elasticApi;
        private readonly ISearchSettings _searchSettings;

        public UserIndexer(IFinder finder, IEntityIndexerIndirect<Client> clientIndexer, IElasticApi elasticApi, ISearchSettings searchSettings)
        {
            _finder = finder;
            _clientIndexer = clientIndexer;
            _elasticApi = elasticApi;
            _searchSettings = searchSettings;
        }

        void IEntityIndexer<User>.IndexEntities(params long[] ids)
        {
            var query = _finder.Find<User>(x => ids.Contains(x.Id));
            var result = GetUserDoc(query);
            _elasticApi.Bulk(result);

            // indirect updates
            var userIds = query.Select(x => x.Id).ToArray();
            var userClients = _finder.FindAll<Client>().Join(userIds, x => x.OwnerCode, x => x, (x, y) => x);
            _clientIndexer.IndexEntitiesIndirectly(userClients);
        }

        void IEntityIndexerIndirect<User>.IndexEntitiesIndirectly(IQueryable<User> query)
        {
            var result = GetUserDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<UserDoc>.IndexAllDocuments()
        {
            var allEntities = _finder.FindAll<User>();
            var result = GetUserDoc(allEntities, true);
            _elasticApi.BulkExclusive<UserDoc>(result);
        }

        private IEnumerable<BulkDescriptor> GetUserDoc(IQueryable<User> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
            {
                x.Id,
                x.DepartmentId,
                x.Department.LeftBorder,
                x.Department.RightBorder,

                x.DisplayName,
                x.Timestamp,

                Permissions = x.UserRoles.Select(y => y.Role).SelectMany(y => y.RolePrivileges)
                    .Where(y => y.Privilege.EntityType == ClientEntityTypeId && y.Privilege.Operation == ReadOperationId)
                    .Select(y => new
                    {
                        y.Mask,
                    }).Distinct(),
            });

            // TODO: optimize
            var departments = _finder.FindAll<Department>().ToArray();

            var indexDescriptors = dtos
                .AsEnumerable()
                .Select(x => new BulkIndexDescriptor<UserDoc>()
                .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                .Version(x.Timestamp, indirectly)
                .Object(new UserDoc
                {
                    Id = x.Id,
                    Name = x.DisplayName,
                    Authorization = new DocumentAuthorization
                    {
                        Tags = x.Permissions.SelectMany(y =>
                            {
                                IEnumerable<string> tags;
                                switch ((EntityPrivilegeDepthState)y.Mask)
                                {
                                    case EntityPrivilegeDepthState.None:
                                        tags = null;
                                        break;
                                    case EntityPrivilegeDepthState.User:
                                        tags = new[] { EntityPrivilegeDepthState.User.ToString().ToLowerInvariant() + '/' + x.Id };
                                        break;
                                    case EntityPrivilegeDepthState.Department:
                                        tags = new[]
                                    {
                                        EntityPrivilegeDepthState.Department.ToString().ToLowerInvariant() + '/' +
                                        x.DepartmentId
                                    };
                                        break;
                                    case EntityPrivilegeDepthState.DepartmentAndChilds:
                                        {
                                            var departmentWord = EntityPrivilegeDepthState.Department.ToString().ToLowerInvariant();

                                            tags = departments.Where(
                                                    z => z.LeftBorder >= x.LeftBorder && z.RightBorder <= x.RightBorder)
                                                    .Select(z => departmentWord + '/' + z.Id);
                                        }

                                        break;
                                    case EntityPrivilegeDepthState.Organization:
                                        tags = new[] { EntityPrivilegeDepthState.Organization.ToString().ToLowerInvariant() };
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                return tags;
                            }),
                    }
                }));

            return indexDescriptors.Batch(_searchSettings.BatchSize).Select(batch =>
            {
                var bulkDescriptor = new BulkDescriptor();
                foreach (var iterator in batch)
                {
                    var indexDescriptor = iterator;
                    bulkDescriptor.Index<UserDoc>(x => indexDescriptor);
                }

                return bulkDescriptor;
            });
        }
    }
}