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
    public class GetReleaseInfoDtoService : GetDomainEntityDtoServiceBase<ReleaseInfo>
    {
        private readonly ISecureFinder _finder;

        public GetReleaseInfoDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<ReleaseInfo> GetDto(long entityId)
        {
            return _finder.Find(new FindSpecification<ReleaseInfo>(x => x.Id == entityId))
                          .Select(entity => new ReleaseInfoDomainEntityDto
                              {
                                  Id = entity.Id,
                                  PeriodStartDate = entity.PeriodStartDate,
                                  PeriodEndDate = entity.PeriodEndDate,
                                  OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                  IsBeta = entity.IsBeta,
                                  Status = entity.Status,
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

        protected override IDomainEntityDto<ReleaseInfo> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new ReleaseInfoDomainEntityDto();
        }
    }
}