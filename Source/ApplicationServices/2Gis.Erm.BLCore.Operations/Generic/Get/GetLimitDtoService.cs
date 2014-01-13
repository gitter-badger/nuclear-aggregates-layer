using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetLimitDtoService : GetDomainEntityDtoServiceBase<Limit>
    {
        private readonly ISecureFinder _finder;
        private readonly IPublicService _publicService;

        public GetLimitDtoService(IUserContext userContext, ISecureFinder finder, IPublicService publicService) 
            : base(userContext)
        {
            _finder = finder;
            _publicService = publicService;
        }

        protected override IDomainEntityDto<Limit> GetDto(long entityId)
        {
            var modelDto = _finder.Find<Limit>(x => x.Id == entityId)
                                  .Select(entity => new LimitDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          AccountRef = new EntityReference { Id = entity.AccountId, Name = null },
                                          Amount = entity.Amount,
                                          LegalPersonRef = new EntityReference()
                                              {
                                                  Id = entity.Account.LegalPersonId,
                                                  Name = entity.Account.LegalPerson.LegalName
                                              },
                                          BranchOfficeRef = new EntityReference()
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
                    var createLimitResponse = (CreateLimitResponse)_publicService.Handle(new CreateLimitRequest { AccountId = accountId });
                    dto = new LimitDomainEntityDto
                    {
                        AccountRef = new EntityReference { Id = accountId },
                        Amount = createLimitResponse.Amount,
                        LegalPersonRef = new EntityReference()
                            {
                                Id = createLimitResponse.LegalPersonId,
                                Name = createLimitResponse.LegalPersonName
                            },
                        BranchOfficeRef = new EntityReference()
                            {
                                Id = createLimitResponse.BranchOfficeId,
                                Name = createLimitResponse.BranchOfficeName
                            },
                        Status = createLimitResponse.Status,
                        StartPeriodDate = createLimitResponse.StartPeriodDate,
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