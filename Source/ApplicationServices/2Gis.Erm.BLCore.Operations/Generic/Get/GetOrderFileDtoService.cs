using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrderFileDtoService : GetDomainEntityDtoServiceBase<OrderFile>
    {
        private readonly ISecureFinder _finder;

        public GetOrderFileDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<OrderFile> GetDto(long entityId)
        {
            return _finder.Find<OrderFile>(x => x.Id == entityId)
                          .Select(entity => new OrderFileDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    OrderId = entity.OrderId,
                                                    FileId = entity.FileId, 
                                                    FileName = entity.File.FileName,
                                                    FileContentLength = entity.File.ContentLength,
                                                    FileContentType = entity.File.ContentType,
                                                    FileKind = (OrderFileKind)entity.FileKind,
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
        }

        protected override IDomainEntityDto<OrderFile> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new OrderFileDomainEntityDto
                {
                    OrderId = parentEntityName == EntityName.Order && parentEntityId.HasValue ? parentEntityId.Value : 0,
                };
        }
    }
}