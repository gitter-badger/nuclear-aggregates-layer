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
    public class GetContributionTypeDtoService : GetDomainEntityDtoServiceBase<ContributionType>
    {
        private readonly ISecureFinder _finder;

        public GetContributionTypeDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<ContributionType> GetDto(long entityId)
        {
            return _finder.Find<ContributionType>(x => x.Id == entityId)
                          .Select(entity => new ContributionTypeDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    Timestamp = entity.Timestamp,
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                                    ModifiedOn = entity.ModifiedOn
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<ContributionType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new ContributionTypeDomainEntityDto();
        }
    }
}