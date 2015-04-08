using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Operations.Generic.Get
{
    public class GetLimitDtoService : GetDomainEntityDtoServiceBase<Limit>
    {
        private readonly ISecureFinder _finder;
        private readonly IAccountReadModel _accountReadModel;

        public GetLimitDtoService(
            IUserContext userContext,
            ISecureFinder finder,
            IAccountReadModel accountReadModel)
            : base(userContext)
        {
            _finder = finder;
            _accountReadModel = accountReadModel;
        }

        protected override IDomainEntityDto<Limit> GetDto(long entityId)
        {
            var modelDto = _finder.Find<Limit>(x => x.Id == entityId)
                                  .Select(entity => new LimitDomainEntityDto
                                  {
                                      Id = entity.Id,
                                      AccountRef = new EntityReference { Id = entity.AccountId, Name = null },
                                      Amount = entity.Amount,
                                      LegalPersonRef = new EntityReference
                                      {
                                          Id = entity.Account.LegalPersonId,
                                          Name = entity.Account.LegalPerson.LegalName
                                      },
                                      BranchOfficeRef = new EntityReference
                                      {
                                          Id = entity.Account.BranchOfficeOrganizationUnit.BranchOfficeId,
                                          Name = entity.Account.BranchOfficeOrganizationUnit.BranchOffice.Name
                                      },
                                      Status = (LimitStatus)entity.Status,
                                      StartPeriodDate = entity.StartPeriodDate,
                                      InspectorRef = new EntityReference
                                      {
                                          Id = entity.InspectorCode,
                                          Name = null
                                      },
                                      Comment = entity.Comment,
                                      CloseDate = entity.CloseDate,
                                      CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                      CreatedOn = entity.CreatedOn,
                                      OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                      IsActive = entity.IsActive,
                                      IsDeleted = entity.IsDeleted,
                                      ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                      ModifiedOn = entity.ModifiedOn,
                                      Timestamp = entity.Timestamp
                                  })
                                  .Single();

            return modelDto;
        }

        protected override IDomainEntityDto<Limit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new LimitDomainEntityDto();
            try
            {
                if (parentEntityName == EntityName.Account && parentEntityId.HasValue)
                {
                    var accountId = parentEntityId.Value;
                    var nextMonthDate = DateTime.UtcNow.Date.AddMonths(1);
                    var periodStart = nextMonthDate.GetFirstDateOfMonth();
                    var periodEnd = nextMonthDate.GetEndPeriodOfThisMonth();

                    var limit = _accountReadModel.InitializeLimitForAccount(accountId);
                    var limitAmount = _accountReadModel.CalculateLimitValueForAccountByPeriod(accountId, periodStart, periodEnd);

                    dto = new LimitDomainEntityDto
                    {
                        AccountRef = new EntityReference { Id = accountId },
                        Amount = limitAmount,
                        LegalPersonRef = new EntityReference
                        {
                            Id = limit.LegalPersonId,
                            Name = limit.LegalPersonName
                        },
                        BranchOfficeRef = new EntityReference
                        {
                            Id = limit.BranchOfficeId,
                            Name = limit.BranchOfficeName
                        },
                        Status = LimitStatus.Opened,
                        StartPeriodDate = periodStart,
                        InspectorRef = new EntityReference()
                    };
                }
            }
            catch (NotificationException ex)
            {
                dto = new LimitDomainEntityDto { ErrorMessage = ex.Message };
            }

            return dto;
        }
    }
}