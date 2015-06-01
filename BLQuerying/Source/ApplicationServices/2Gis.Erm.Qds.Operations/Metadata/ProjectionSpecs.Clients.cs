using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        public static class Clients
        {
            public static SelectSpecification<Client, object> Select()
            {
                return new SelectSpecification<Client, object>(
                    x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.TerritoryId,
                                 x.OwnerCode,
                                 x.MainFirmId,
                                 x.MainAddress,
                                 x.IsAdvertisingAgency,
                                 x.MainPhoneNumber,
                                 x.CreatedOn,
                                 x.LastQualifyTime,
                                 x.LastDisqualifyTime,
                                 x.IsActive,
                                 x.IsDeleted,
                                 x.InformationSource
                             });
            }

            public static MapSpecification<ObjectAccessor, IndexedDocumentWrapper<ClientGridDoc>> Project()
            {
                return new MapSpecification<ObjectAccessor, IndexedDocumentWrapper<ClientGridDoc>>(
                    x =>
                        {
                            var accessor = x.BasedOn<Client>();
                            return new IndexedDocumentWrapper<ClientGridDoc>
                                       {
                                           Id = accessor.Get(c => c.Id).ToString(),
                                           Document = new ClientGridDoc
                                                          {
                                                              Id = accessor.Get(c => c.Id),
                                                              Name = accessor.Get(c => c.Name),
                                                              MainAddress = accessor.Get(c => c.MainAddress),
                                                              IsAdvertisingAgency = accessor.Get(c => c.IsAdvertisingAgency),
                                                              MainPhoneNumber = accessor.Get(c => c.MainPhoneNumber),
                                                              CreatedOn = accessor.Get(c => c.CreatedOn),
                                                              LastQualifyTime = accessor.Get(c => c.LastQualifyTime),
                                                              LastDisqualifyTime = accessor.Get(c => c.LastDisqualifyTime),
                                                              IsActive = accessor.Get(c => c.IsActive),
                                                              IsDeleted = accessor.Get(c => c.IsDeleted),
                                                              InformationSourceEnum = accessor.Get(c => c.InformationSource),

                                                              // relations
                                                              TerritoryId = GetRelatedId(accessor.Get(c => c.TerritoryId)),
                                                              OwnerCode = GetRelatedId(accessor.Get(c => c.OwnerCode)),
                                                              MainFirmId = GetRelatedId(accessor.Get(c => c.MainFirmId))
                                                          }
                                       };
                        });
            }
        }
    }
}