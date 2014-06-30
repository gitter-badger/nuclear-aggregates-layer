using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class UserIndexer :
        IEntityIndexer<User>,
        IEntityIndexerIndirect<User>,
        IDocumentIndexer<UserDoc>
    {
        private readonly IFinder _finder;
        private readonly IEntityIndexerIndirect<Client> _clientIndexer;
        private readonly IElasticApi _elasticApi;

        public UserIndexer(IFinder finder, IEntityIndexerIndirect<Client> clientIndexer, IElasticApi elasticApi)
        {
            _finder = finder;
            _clientIndexer = clientIndexer;
            _elasticApi = elasticApi;
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
            var result = GetUserDoc(allEntities);
            _elasticApi.Bulk(result);
            _elasticApi.Refresh(x => x.Index<UserDoc>());
        }

        private IEnumerable<Func<BulkDescriptor, BulkDescriptor>> GetUserDoc(IQueryable<User> query, bool indirectly = false)
        {
            // TODO {m.pashuk, 24.03.2014}: пока индексируем только операции чтения
            const int ReadEntityOperationId = 1;

            // TODO {m.pashuk, 24.03.2014}: пока индексируем только привилегии клиента и фирмы
            var allowedEntities = new[]
            {
                EntityName.Client,
                EntityName.Firm
            };

            var dtos = query.Select(x => new
            {
                x.Id,
                x.DepartmentId,
                x.Department.LeftBorder,
                x.Department.RightBorder,

                x.DisplayName,
                x.Timestamp,

                TerritoryIds = x.UserTerritories.Where(y => !y.IsDeleted).Select(y => y.TerritoryId).Distinct(),
                OrganizationUnitIds = x.UserOrganizationUnits.Select(y => y.OrganizationUnitId).Distinct(),

                Permissions = x.UserRoles.Select(y => y.Role).SelectMany(y => y.RolePrivileges)
                    .Where(y => y.Privilege.Operation == ReadEntityOperationId)
                    .Select(y => new
                    {
                        EntityName = (EntityName)y.Privilege.EntityType.Value,
                        Mask = (EntityPrivilegeDepthState)y.Mask,
                    })
                    .Distinct()
                    .Where(y => y.Mask != EntityPrivilegeDepthState.None)
                    .Where(y => allowedEntities.Contains(y.EntityName)),
            });

            // TODO: optimize
            var departments = _finder.FindAll<Department>().ToArray();

            var bulkDescriptors = dtos
                .AsEnumerable()
                .Select(x =>
                {
                    var entityPermissions = x.Permissions.GroupBy(y => y.EntityName, y => y.Mask).SelectMany(y =>
                    {
                        Type documentType;
                        if (!DocToEntityMappingMetadata.TryGetGridDocType(y.Key.AsEntityType(), out documentType))
                        {
                            throw new ArgumentException();
                        }

                        return y.Select(z =>
                        {
                            switch (z)
                            {
                                case EntityPrivilegeDepthState.User:
                                    return new OperationPermission
                                        {
                                            Operation = "list/" + documentType.Name,
                                            Tags = new[] { "byUserId/" + x.Id }
                                        };

                                case EntityPrivilegeDepthState.Department:
                                    return new OperationPermission
                                    {
                                        Operation = "list/" + documentType.Name,
                                        Tags = new[] { "byDepartmentId/" + x.DepartmentId }
                                    };

                                case EntityPrivilegeDepthState.DepartmentAndChilds:
                                    {
                                        var childDepartmentsIds = departments.Where(p => p.LeftBorder >= x.LeftBorder && p.RightBorder <= x.RightBorder).Select(p => "byDepartmentId/" + p.Id);

                                        return new OperationPermission
                                        {
                                            Operation = "list/" + documentType.Name,
                                            Tags = childDepartmentsIds,
                                        };
                                    }
                                case EntityPrivilegeDepthState.Organization:
                                    return new OperationPermission
                                    {
                                        Operation = "list/" + documentType.Name,
                                    };
                                default:
                                    throw new ArgumentException();
                            }
                        });
                    });

                    var segmentTags = x.TerritoryIds.Select(y => "byTerritoryId/" + y)
                                       .Concat(x.OrganizationUnitIds.Select(y => "byOrganizationUnitId/" + y)).ToArray();
                    if (segmentTags.Any())
                    {
                        entityPermissions = entityPermissions.Concat(new []
                        {
                            new OperationPermission
                            {
                                Operation = "list",
                                Tags = segmentTags,
                            }  
                        });
                    }

                    return new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor => bulkDescriptor
                        .Index<UserDoc>(bulkIndexDescriptor => bulkIndexDescriptor
                            .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                            .Object(new UserDoc
                            {
                                Id = x.Id.ToString(),
                                Name = x.DisplayName,
                                Permissions = entityPermissions,
                            })
                    ));
                });

            return bulkDescriptors;
        }
    }
}