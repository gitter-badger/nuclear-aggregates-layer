using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetCategoryGroupDtoService : GetDomainEntityDtoServiceBase<CategoryGroup>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetCategoryGroupDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<CategoryGroup> GetDto(long entityId)
        {
            return _finder.Find<CategoryGroup>(x => x.Id == entityId)
                          .Select(entity => new CategoryGroupDomainEntityDto
                              {
                                  Id = entity.Id,
                                  CategoryGroupName = entity.CategoryGroupName,
                                  GroupRate = entity.GroupRate,
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

        protected override IDomainEntityDto<CategoryGroup> CreateDto(long? parentId, EntityName parentEntityType, string extendedInfo)
        {
            return new CategoryGroupDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}