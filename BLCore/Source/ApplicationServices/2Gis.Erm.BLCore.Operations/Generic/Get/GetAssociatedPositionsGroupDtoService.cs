using System;
using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAssociatedPositionsGroupDtoService : GetDomainEntityDtoServiceBase<AssociatedPositionsGroup>
    {
        private readonly ISecureFinder _finder;

        public GetAssociatedPositionsGroupDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AssociatedPositionsGroup> GetDto(long entityId)
        {
            return _finder.Find<AssociatedPositionsGroup>(x => x.Id == entityId)
                          .Select(entity => new AssociatedPositionsGroupDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  PriceIsDeleted = entity.PricePosition.Price.IsDeleted,
                                  PriceIsPublished = entity.PricePosition.Price.IsPublished,
                                  PricePositionRef = new EntityReference { Id = entity.PricePositionId, Name = entity.PricePosition.Position.Name },
                                  OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<AssociatedPositionsGroup> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (!parentEntityId.HasValue)
            {
                throw new ArgumentNullException("parentEntityId");
            }

            if (parentEntityName != EntityName.PricePosition)
            {
                throw new NotSupportedException("Only PricePosition parent type is supported");
            }

            return _finder.Find<PricePosition>(x => x.Id == parentEntityId)
                          .Select(x => new AssociatedPositionsGroupDomainEntityDto
                              {
                                  PricePositionRef = new EntityReference { Id = x.Id, Name = x.Position.Name },
                                  PriceIsPublished = x.Price.IsPublished,
                                  OwnerRef = new EntityReference() {Id = x.OwnerCode, Name = null}
                              })
                          .Single();
        }
    }
}