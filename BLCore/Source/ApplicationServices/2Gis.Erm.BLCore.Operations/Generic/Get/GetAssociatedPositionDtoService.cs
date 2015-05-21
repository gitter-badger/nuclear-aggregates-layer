using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAssociatedPositionDtoService : GetDomainEntityDtoServiceBase<AssociatedPosition>
    {
        private readonly ISecureFinder _finder;

        public GetAssociatedPositionDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AssociatedPosition> GetDto(long entityId)
        {
            return _finder.Find(new FindSpecification<AssociatedPosition>(x => x.Id == entityId))
                          .Select(entity => new AssociatedPositionDomainEntityDto
                              {
                                  Id = entity.Id,
                                  PositionRef = new EntityReference { Id = entity.PositionId, Name = entity.Position.Name },
                                  ObjectBindingType = entity.ObjectBindingType,
                                  AssociatedPositionsGroupRef = new EntityReference { Id = entity.AssociatedPositionsGroupId, Name = entity.AssociatedPositionsGroup.Name },
                                  PricePositionRef = new EntityReference { Id = entity.AssociatedPositionsGroup.PricePositionId, Name = entity.AssociatedPositionsGroup.PricePosition.Position.Name },
                                  IsDeleted = entity.IsDeleted,
                                  PriceIsPublished = entity.AssociatedPositionsGroup.PricePosition.Price.IsPublished,
                                  PriceIsDeleted = entity.AssociatedPositionsGroup.PricePosition.Price.IsDeleted,
                                  OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<AssociatedPosition> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (!parentEntityId.HasValue)
            {
                throw new ArgumentNullException("parentEntityId");
            }

            if (!parentEntityName.Equals(EntityType.Instance.AssociatedPositionsGroup()))
            {
                throw new NotSupportedException("Only AssociatedPositionsGroup parent type is supported");
            }

            return _finder.Find(new FindSpecification<AssociatedPositionsGroup>(x => x.Id == parentEntityId))
                          .Select(x => new AssociatedPositionDomainEntityDto
                              {
                                  AssociatedPositionsGroupRef = new EntityReference { Id = parentEntityId.Value, Name = x.Name },
                                  PricePositionRef = new EntityReference { Id = x.PricePositionId, Name = x.PricePosition.Position.Name },
                                  PriceIsPublished = x.PricePosition.Price.IsPublished,
                              })
                          .Single();
        }
    }
}