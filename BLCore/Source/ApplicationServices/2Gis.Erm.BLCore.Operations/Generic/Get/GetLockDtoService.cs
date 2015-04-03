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
    public class GetLockDtoService : GetDomainEntityDtoServiceBase<Lock>
    {
        private readonly ISecureFinder _finder;

        public GetLockDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Lock> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new LockDomainEntityDto { IsActive = true };
        }

        protected override IDomainEntityDto<Lock> GetDto(long entityId)
        {
            var dto = _finder.Find<Lock>(x => x.Id == entityId)
                          .Select(entity => new LockDomainEntityDto
                              {
                                  Id = entity.Id,
                                  AccountRef = new EntityReference { Id = entity.AccountId, Name = null },
                                  BranchOfficeOrganizationUnitRef = new EntityReference
                                      {
                                          Id = entity.Account.BranchOfficeOrganizationUnitId,
                                          Name = entity.Account.BranchOfficeOrganizationUnit.ShortLegalName
                                      },
                                  LegalPersonRef = new EntityReference
                                      {
                                          Id = entity.Account.LegalPersonId,
                                          Name = entity.Account.LegalPerson.LegalName
                                      },
                                  DebitAccountDetailRef = new EntityReference
                                      {
                                          Id = entity.DebitAccountDetailId,
                                          Name = entity.AccountDetail.OperationType.Name
                                      },

                                  OrderId = entity.OrderId,
                                  OrderRef = new EntityReference
                                      {
                                          Id = entity.OrderId,
                                          Name = entity.Order.Number
                                      },
                                 
                                  PeriodStartDate = entity.PeriodStartDate,
                                  PeriodEndDate = entity.PeriodEndDate,
                                  CloseDate = entity.CloseDate,
                                  Balance = entity.Balance,
                                  PlannedAmount = entity.PlannedAmount,
                                  ClosedBalance = entity.ClosedBalance,
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
            return dto;
        }
    }
}