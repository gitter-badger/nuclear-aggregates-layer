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
        public static class Countries
        {
            public static SelectSpecification<Country, object> Select()
            {
                return new SelectSpecification<Country, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.IsoCode,
                                 x.IsDeleted,
                                 x.CurrencyId
                             });
            }

            public static ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<CountryGridDoc>> Project()
            {
                return new ProjectSpecification<ObjectAccessor, IndexedDocumentWrapper<CountryGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Country>();
                            return new IndexedDocumentWrapper<CountryGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new CountryGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              IsoCode = long.Parse(accessor.Get(c => c.IsoCode)),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              CurrencyId = GetRelatedId(accessor.Get(c => c.CurrencyId))
                                                          }
                                       };
                        });
            }
        }
    }
}