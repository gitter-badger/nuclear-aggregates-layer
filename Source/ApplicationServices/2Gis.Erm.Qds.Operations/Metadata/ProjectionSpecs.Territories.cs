using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Territories
        {
            public static ISelectSpecification<Territory, object> Select()
            {
                return new SelectSpecification<Territory, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.OrganizationUnitId,
                                 x.IsActive
                             });
            }

            public static IProjectSpecification<ObjectAccessor, DocumentWrapper<TerritoryGridDoc>> Project()
            {
                return new ProjectSpecification<ObjectAccessor, DocumentWrapper<TerritoryGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Territory>();
                            return new DocumentWrapper<TerritoryGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new TerritoryGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              IsActive = accessor.Get(c => c.IsActive),

                                                              // relations
                                                              OrganizationUnitId = GetRelatedId(accessor.Get(c => c.OrganizationUnitId))
                                                          }
                                       };
                        });
            }
        }
    }
}