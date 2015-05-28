using System.Globalization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
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
        public static class Bargains
        {
            public static SelectSpecification<Bargain, object> Select()
            {
                return new SelectSpecification<Bargain, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Number,
                                 x.CreatedOn,
                                 x.BargainEndDate,
                                 x.BargainKind,
                                 x.IsActive,
                                 x.IsDeleted,
                                 x.CustomerLegalPersonId,
                                 x.OwnerCode,
                                 x.LegalPerson.ClientId
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<BargainGridDoc>> Project(CultureInfo cultureInfo)
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<BargainGridDoc>>(x =>
                {
                    var accessor = x.BasedOn<Bargain>();
                    var bargainKind = accessor.Get(c => c.BargainKind);
                    return new IndexedDocumentWrapper<BargainGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new BargainGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Number = accessor.Get(c => c.Number),
                                                              BargainKindEnum = bargainKind,
                                                              BargainKind = bargainKind.ToStringLocalized(EnumResources.ResourceManager, cultureInfo),
                                                              BargainEndDate = accessor.Get(c => c.BargainEndDate),
                                                              CreatedOn = accessor.Get(c => c.CreatedOn),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),

                                                              // relations
                                                              OwnerCode = GetRelatedId(accessor.Get(c => c.OwnerCode)),
                                                              LegalPersonId = GetRelatedId(accessor.Get(c => c.CustomerLegalPersonId)),
                                                              ClientId = GetRelatedId(accessor.Get(c => c.LegalPerson.ClientId))
                                                          }
                                       };
                        });
            }
        }
    }
}