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
    public class GetCategoryDtoService : GetDomainEntityDtoServiceBase<Category>
    {
        private readonly ISecureFinder _finder;

        public GetCategoryDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Category> GetDto(long entityId)
        {
            return _finder.Find<Category>(x => x.Id == entityId)
                          .Select(entity => new CategoryDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  Level = entity.Level,
                                  ParentRef = new EntityReference { Id = entity.ParentId, Name = entity.ParentCategory.Name },
                                  Comment = entity.Comment,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Category> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new CategoryDomainEntityDto();
        }
    }
}