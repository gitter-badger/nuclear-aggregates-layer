using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetNoteDtoService : GetDomainEntityDtoServiceBase<Note>
    {
        private readonly ISecureFinder _finder;

        public GetNoteDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Note> GetDto(long entityId)
        {
            var result = _finder.Find<Note>(x => x.Id == entityId)
                                .Select(entity => new
                                    {
                                        Dto = new NoteDomainEntityDto
                                            {
                                                Id = entity.Id,
                                                ParentRef = new EntityReference { Id = entity.ParentId },
                                                Title = entity.Title,
                                                Text = entity.Text,
                                                FileId = entity.FileId,
                                                FileName = entity.File.FileName,
                                                FileContentLength = entity.File != null ? entity.File.ContentLength : 0,
                                                FileContentType = entity.File.ContentType,
                                                OwnerRef = new EntityReference { Id = entity.OwnerCode },
                                                CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                                CreatedOn = entity.CreatedOn,
                                                IsDeleted = entity.IsDeleted,
                                                ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                                ModifiedOn = entity.ModifiedOn,
                                                Timestamp = entity.Timestamp
                                            },
                                        entity.ParentType
                                    })
                                .Single();
            result.Dto.ParentTypeName = EntityType.Instance.Parse(result.ParentType);
            
            return result.Dto;
        }

        protected override IDomainEntityDto<Note> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new NoteDomainEntityDto();
        }
    }
}