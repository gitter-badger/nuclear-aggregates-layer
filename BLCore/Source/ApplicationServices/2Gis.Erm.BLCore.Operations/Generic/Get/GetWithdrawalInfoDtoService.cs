using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetWithdrawalInfoDtoService : GetDomainEntityDtoServiceBase<WithdrawalInfo>
    {
        private readonly ISecureFinder _finder;

        public GetWithdrawalInfoDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<WithdrawalInfo> GetDto(long entityId)
        {
            return _finder.Find<WithdrawalInfo>(x => x.Id == entityId)
                          .Select(entity => new WithdrawalInfoDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    PeriodStartDate = entity.PeriodStartDate,
                                                    PeriodEndDate = entity.PeriodEndDate,
                                                    OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
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

        protected override IDomainEntityDto<WithdrawalInfo> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new WithdrawalInfoDomainEntityDto();
        }
    }
}