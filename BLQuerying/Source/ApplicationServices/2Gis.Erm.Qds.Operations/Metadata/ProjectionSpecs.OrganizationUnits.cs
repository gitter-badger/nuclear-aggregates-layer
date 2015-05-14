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
        public static class OrganizationUnits
        {
            public static SelectSpecification<OrganizationUnit, object> Select()
            {
                return new SelectSpecification<OrganizationUnit, object>(
                    x => new
                             {
                                 x.Id,
                                 x.DgppId,
                                 x.ReplicationCode,
                                 x.CountryId,
                                 x.Name,
                                 x.FirstEmitDate,
                                 x.ErmLaunchDate,
                                 x.InfoRussiaLaunchDate,
                                 x.IsActive,
                                 x.IsDeleted
                             });
            }

            public static IProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<OrgUnitGridDoc>> Project()
            {
                return new ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<OrgUnitGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<OrganizationUnit>();
                            return new IndexedDocumentWrapper<OrgUnitGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new OrgUnitGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              DgppId = accessor.Get(c => c.DgppId),
                                                              ReplicationCode = accessor.Get(c => c.ReplicationCode).ToString(),
                                                              Name = accessor.Get(c => c.Name),

                                                              FirstEmitDate = accessor.Get(c => c.FirstEmitDate),
                                                              ErmLaunchDate = accessor.Get(c => c.ErmLaunchDate),
                                                              InfoRussiaLaunchDate = accessor.Get(c => c.InfoRussiaLaunchDate),

                                                              ErmLaunched = accessor.Get(c => c.ErmLaunchDate) != null,
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              CountryId = GetRelatedId(accessor.Get(c => c.CountryId))
                                                          }
                                       };
                        });
            }
        }
    }
}