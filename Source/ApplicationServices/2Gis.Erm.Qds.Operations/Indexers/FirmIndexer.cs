using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class FirmIndexer :
        IEntityIndexer<Firm>,
        IEntityIndexerIndirect<Firm>,
        IDocumentIndexer<FirmGridDoc>
    {
        private readonly IFinder _finder;
        private readonly IElasticApi _elasticApi;

        public FirmIndexer(IFinder finder, IElasticApi elasticApi)
        {
            _finder = finder;
            _elasticApi = elasticApi;
        }

        void IEntityIndexer<Firm>.IndexEntities(params long[] ids)
        {
            var firms = _finder.Find<Firm>(x => ids.Contains(x.Id));
            var result = GetFirmGridDoc(firms);
            _elasticApi.Bulk(result);
        }

        void IEntityIndexerIndirect<Firm>.IndexEntitiesIndirectly(IQueryable<Firm> query)
        {
            var result = GetFirmGridDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<FirmGridDoc>.IndexAllDocuments()
        {
            var firms = _finder.FindAll<Firm>();
            var result = GetFirmGridDoc(firms);
            _elasticApi.Bulk(result);
            _elasticApi.Refresh(x => x.Index<FirmGridDoc>());
        }

        private IEnumerable<Func<BulkDescriptor, BulkDescriptor>> GetFirmGridDoc(IQueryable<Firm> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
            {
                x.Id, 
                x.Name,
                x.ClientId,
                ClientName = x.Client.Name,
                x.OwnerCode,
                x.TerritoryId, 
                TerritoryName = x.Territory.Name, 
                x.PromisingScore,
                x.LastQualifyTime,
                x.LastDisqualifyTime,
                x.OrganizationUnitId,
                OrganizationUnitName = x.OrganizationUnit.Name,
                x.IsActive, 
                x.IsDeleted, 
                x.ClosedForAscertainment,
                x.Timestamp,
            });

            // TODO: optimize
            var allUsers = _finder.FindAll<User>().ToArray();

            var bulkDescriptors = dtos
                .AsEnumerable()
                .Join(
                allUsers, 
                x => x.OwnerCode, 
                x => x.Id,
                (x, y) => new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor => bulkDescriptor
                            .Index<FirmGridDoc>(bulkIndexDescriptor => bulkIndexDescriptor
                                .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                                .Object(new FirmGridDoc
                                {
                                    Id = x.Id.ToString(),
                                    Name = x.Name,
                                    ClientId = x.ClientId.ToString(),
                                    ClientName = x.ClientName,
                                    OwnerCode = x.OwnerCode.ToString(),
                                    OwnerName = y.DisplayName,
                                    TerritoryId = x.TerritoryId.ToString(),
                                    TerritoryName = x.TerritoryName, 
                                    PromisingScore = x.PromisingScore,
                                    LastQualifyTime = x.LastQualifyTime,
                                    LastDisqualifyTime = x.LastDisqualifyTime,
                                    OrganizationUnitId = x.OrganizationUnitId.ToString(),
                                    OrganizationUnitName = x.OrganizationUnitName,
                                    IsActive = x.IsActive,
                                    IsDeleted = x.IsDeleted, 
                                    ClosedForAscertainment = x.ClosedForAscertainment,

                                    Authorization = new DocumentAuthorization
                                    {
                                        Tags = new[]
                                        {
                                            "list/" + typeof(FirmGridDoc).Name,
                                            "byUserId/" + x.OwnerCode,
                                            "byDepartmentId/" + y.DepartmentId,
                                            "byTerritoryId/" + x.TerritoryId,
                                            "byOrganizationUnitId/" + x.OrganizationUnitId
                                        },
                                    },
                                })
                            )
                        )
                );

            return bulkDescriptors;
        }
    }
}