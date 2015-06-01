using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Currencies
        {
            public static SelectSpecification<Currency, object> Select()
            {
                return new SelectSpecification<Currency, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.ISOCode,
                                 x.Symbol,
                                 x.IsBase,
                                 x.IsActive,
                                 x.IsDeleted
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<CurrencyGridDoc>> Project()
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<CurrencyGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Currency>();
                            return new IndexedDocumentWrapper<CurrencyGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new CurrencyGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              IsoCode = accessor.Get(c => c.ISOCode),
                                                              Symbol = accessor.Get(c => c.Symbol),
                                                              IsBase = accessor.Get(c => c.IsBase),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),
                                                          },
                                       };
                        });
            }
        }
    }
}