using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Firms
        {
            public static SelectSpecification<Firm, object> Select()
            {
                return new SelectSpecification<Firm, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.PromisingScore,
                                 x.LastQualifyTime,
                                 x.LastDisqualifyTime,
                                 x.IsActive,
                                 x.IsDeleted,
                                 x.ClosedForAscertainment,
                                 x.ClientId,
                                 x.OwnerCode,
                                 x.TerritoryId,
                                 x.OrganizationUnitId
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<FirmGridDoc>> Project()
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<FirmGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Firm>();
                            return new IndexedDocumentWrapper<FirmGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new FirmGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              PromisingScore = accessor.Get(c => c.PromisingScore),
                                                              LastQualifyTime = accessor.Get(c => c.LastQualifyTime),
                                                              LastDisqualifyTime = accessor.Get(c => c.LastDisqualifyTime),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),
                                                              ClosedForAscertainment = accessor.Get(c => c.ClosedForAscertainment),

                                                              // relations
                                                              ClientId = GetRelatedId(accessor.Get(c => c.ClientId)),
                                                              OwnerCode = GetRelatedId(accessor.Get(c => c.OwnerCode)),
                                                              TerritoryId = GetRelatedId(accessor.Get(c => c.TerritoryId)),
                                                              OrganizationUnitId = GetRelatedId(accessor.Get(c => c.OrganizationUnitId))
                                                          }
                                       };
                        });
            }
        }
    }
}