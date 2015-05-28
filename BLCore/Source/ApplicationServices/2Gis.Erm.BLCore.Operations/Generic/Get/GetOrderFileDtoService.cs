using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrderFileDtoService : GetDomainEntityDtoServiceBase<OrderFile>
    {
        private readonly IUserContext _userContext;
        private readonly ISecureFinder _finder;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceEntityAccessInternal _securityServiceEntityAccess;

        public GetOrderFileDtoService(IUserContext userContext,
                                      ISecureFinder finder,
                                      IOrderReadModel orderReadModel,
                                      ISecurityServiceEntityAccessInternal securityServiceEntityAccess) 
            : base(userContext)
        {
            _userContext = userContext;
            _finder = finder;
            _orderReadModel = orderReadModel;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        protected override IDomainEntityDto<OrderFile> GetDto(long entityId)
        {
            var dto = _finder.FindObsolete(new FindSpecification<OrderFile>(x => x.Id == entityId))
                             .Select(entity => new OrderFileDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     OrderId = entity.OrderId,
                                     FileId = entity.FileId,
                                     FileName = entity.File.FileName,
                                     FileContentLength = entity.File.ContentLength,
                                     FileContentType = entity.File.ContentType,
                                     FileKind = entity.FileKind,
                                     Comment = entity.Comment,
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

            var orderOwnerCode = _orderReadModel.GetOrderOwnerCode(dto.OrderId);

            dto.SetReadonly = !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                                EntityType.Instance.Order(),
                                                                                                _userContext.Identity.Code,
                                                                                                dto.OrderId,
                                                                                                orderOwnerCode,
                                                                                                orderOwnerCode);
            return dto;
        }

        protected override IDomainEntityDto<OrderFile> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new OrderFileDomainEntityDto
                {
                    OrderId = parentEntityName.Equals(EntityType.Instance.Order()) && parentEntityId.HasValue ? parentEntityId.Value : 0,
                };
        }
    }
}