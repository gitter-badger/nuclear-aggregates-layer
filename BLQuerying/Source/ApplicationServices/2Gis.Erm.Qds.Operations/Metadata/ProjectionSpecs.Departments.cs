using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Departments
        {
            public static SelectSpecification<Department, object> Select()
            {
                return new SelectSpecification<Department, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.ParentId,
                                 x.IsActive,
                                 x.IsDeleted
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<DepartmentGridDoc>> Project()
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<DepartmentGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Department>();
                            return new IndexedDocumentWrapper<DepartmentGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new DepartmentGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              ParentId = GetRelatedId(accessor.Get(c => c.ParentId))
                                                          }
                                       };
                        });
            }
        }
    }
}