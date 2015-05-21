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
    public class GetPositionCategoryDtoService : GetDomainEntityDtoServiceBase<PositionCategory>
    {
        private readonly ISecureFinder _finder;

        public GetPositionCategoryDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<PositionCategory> GetDto(long entityId)
        {
            return _finder.Find(new FindSpecification<PositionCategory>(x => x.Id == entityId))
                          .Select(entity => new PositionCategoryDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    ExportCode = entity.ExportCode,
                                                    Timestamp = entity.Timestamp,
                                                    IsSupportedByExport = entity.IsSupportedByExport,
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                                    ModifiedOn = entity.ModifiedOn
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<PositionCategory> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new PositionCategoryDomainEntityDto();
        }
    }
}