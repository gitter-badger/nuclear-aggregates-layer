using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class LegalPersons
        {
            public static SelectSpecification<LegalPerson, object> Select()
            {
                return new SelectSpecification<LegalPerson, object>(
                    x => new
                             {
                                 x.Id,
                                 x.LegalName,
                                 x.ShortName,
                                 x.Inn,
                                 x.Kpp,
                                 x.LegalAddress,
                                 x.PassportNumber,
                                 x.CreatedOn,
                                 x.IsActive,
                                 x.IsDeleted,
                                 x.OwnerCode,
                                 x.ClientId
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<LegalPersonGridDoc>> Project()
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<LegalPersonGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<LegalPerson>();
                            return new IndexedDocumentWrapper<LegalPersonGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new LegalPersonGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              LegalName = accessor.Get(c => c.LegalName),
                                                              ShortName = accessor.Get(c => c.ShortName),
                                                              Inn = accessor.Get(c => c.Inn),
                                                              Kpp = accessor.Get(c => c.Kpp),
                                                              LegalAddress = accessor.Get(c => c.LegalAddress),
                                                              PassportNumber = accessor.Get(c => c.PassportNumber),
                                                              CreatedOn = accessor.Get(c => c.CreatedOn),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              OwnerCode = GetRelatedId(accessor.Get(c => c.OwnerCode)),
                                                              ClientId = GetRelatedId(accessor.Get(c => c.ClientId)),
                                                          }
                                       };
                        });
            }
        }
    }
}