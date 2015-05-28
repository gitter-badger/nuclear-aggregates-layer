using System.Linq;

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
    public class GetBargainFileDtoService : GetDomainEntityDtoServiceBase<BargainFile>
    {
        private readonly ISecureFinder _finder;

        public GetBargainFileDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<BargainFile> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<BargainFile>(x => x.Id == entityId))
                          .Select(entity => new BargainFileDomainEntityDto
                              {
                                  Id = entity.Id,
                                  BargainRef = new EntityReference { Id = entity.BargainId, Name = null },
                                  FileId = entity.FileId, 
                                  FileName = entity.File.FileName,
                                  FileContentType = entity.File.ContentType,
                                  FileContentLength = entity.File.ContentLength,
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
        }

        protected override IDomainEntityDto<BargainFile> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new BargainFileDomainEntityDto
                {
                    BargainRef = new EntityReference { Id = (parentEntityName == EntityType.Instance.Bargain() && parentEntityId.HasValue) ? parentEntityId.Value : 0 }
                };
        }
    }
}