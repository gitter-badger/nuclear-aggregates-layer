using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetLockDetailDtoService : GetDomainEntityDtoServiceBase<LockDetail>
    {
        private readonly ISecureFinder _finder;

        public GetLockDetailDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<LockDetail> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new LockDetailDomainEntityDto();
        }

        protected override IDomainEntityDto<LockDetail> GetDto(long entityId)
        {
            var dto = _finder.FindObsolete(new FindSpecification<LockDetail>(x => x.Id == entityId))
                             .Select(entity => new LockDetailDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     Amount = entity.Amount,
                                     PriceRef = new EntityReference { Id = entity.PriceId },
                                     Description = entity.Description,
                                     LockRef = new EntityReference { Id = entity.LockId },
                                     OwnerRef = new EntityReference { Id = entity.OwnerCode },
                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                     CreatedOn = entity.CreatedOn,
                                     IsActive = entity.IsActive,
                                     IsDeleted = entity.IsDeleted,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                     OrderPositionRef = new EntityReference { Id = entity.OrderPositionId },
                                     ModifiedOn = entity.ModifiedOn,
                                     Timestamp = entity.Timestamp
                                 })
                             .Single();

            if (dto.OrderPositionRef.Id != null)
            {
                dto.OrderPositionRef.Name = _finder.FindObsolete(Specs.Find.ById<OrderPosition>(dto.OrderPositionRef.Id.Value))
                                                   .Select(x => x.PricePosition.Position.Name)
                                                   .Single();
            }

            return dto;
        }
    }
}