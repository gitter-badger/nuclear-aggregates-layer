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
        public static class Countries
        {
            public static ISelectSpecification<Country, object> Select()
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

            public static IProjectSpecification<ObjectAccessor, DocumentWrapper<CountryGridDoc>> Project()
            {
                return new ProjectSpecification<ObjectAccessor, DocumentWrapper<CountryGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Country>();
                            return new DocumentWrapper<CountryGridDoc>
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