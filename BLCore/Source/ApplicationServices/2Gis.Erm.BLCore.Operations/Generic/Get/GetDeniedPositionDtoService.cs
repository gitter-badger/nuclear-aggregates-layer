using System;
using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetDeniedPositionDtoService : GetDomainEntityDtoServiceBase<DeniedPosition>
    {
        private readonly ISecureFinder _finder;

        public GetDeniedPositionDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<DeniedPosition> GetDto(long entityId)
        {
            var positionQuery = _finder.FindAll<Position>();
            return (from deniesPosition in _finder.Find<DeniedPosition>(x => x.Id == entityId)
                    join position in positionQuery on deniesPosition.PositionId equals position.Id
                    select new DeniedPositionDomainEntityDto
                        {
                            Id = deniesPosition.Id,
                            PositionRef = new EntityReference { Id = deniesPosition.PositionId, Name = position.Name },
                            PositionDeniedRef = new EntityReference { Id = deniesPosition.PositionDeniedId, Name = deniesPosition.PositionDenied.Name },
                            PriceRef = new EntityReference { Id = deniesPosition.PriceId, Name = null },
                            PriceIsPublished = deniesPosition.Price.IsPublished,
                            ObjectBindingType = deniesPosition.ObjectBindingType,
                            OwnerRef = new EntityReference { Id = deniesPosition.OwnerCode, Name = null },
                            CreatedByRef = new EntityReference { Id = deniesPosition.CreatedBy, Name = null },
                            CreatedOn = deniesPosition.CreatedOn,
                            IsActive = deniesPosition.IsActive,
                            IsDeleted = deniesPosition.IsDeleted,
                            ModifiedByRef = new EntityReference { Id = deniesPosition.ModifiedBy, Name = null },
                            ModifiedOn = deniesPosition.ModifiedOn,
                            Timestamp = deniesPosition.Timestamp
                        })
                .Single();
        }

        protected override IDomainEntityDto<DeniedPosition> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            long pricePositionsId;
            if (parentEntityName.Equals(EntityType.Instance.PricePosition()) && parentEntityId.HasValue)
            {
                pricePositionsId = parentEntityId.Value;
            }
            else
            {
                throw new ArgumentNullException("parentEntityId");
            }

            var model = _finder.Find<PricePosition>(x => x.Id == pricePositionsId)
                               .Select(x => new DeniedPositionDomainEntityDto
                                   {
                                       PriceRef = new EntityReference { Id = x.PriceId, Name = null },
                                       PositionRef = new EntityReference { Id = x.PositionId, Name = x.Position.Name },
                                       PriceIsPublished = x.Price.IsPublished,
                                       OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null }
                                   })
                               .Single();

            return model;
        }
    }
}