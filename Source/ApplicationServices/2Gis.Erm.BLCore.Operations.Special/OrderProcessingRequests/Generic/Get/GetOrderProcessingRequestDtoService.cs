using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Generic.Get
{
    // FIXME {all, 06.11.2013}: ��� merge �� 1.0 ������ ��� �������� � BL.Operations ��� ������������� ��������� OrderProcessingRequest ����� ��� ��������� � Operation.Special, ������ ������� ����� ���������� ��  BL.Operations - ��� �������� �� ����� ������
    public class GetOrderProcessingRequestDtoService : GetDomainEntityDtoServiceBase<Platform.Model.Entities.Erm.OrderProcessingRequest>
    {
        private readonly IUserContext _userContext;
        private readonly ISecureFinder _finder;

        public GetOrderProcessingRequestDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _userContext = userContext;
            _finder = finder;
        }

        protected override IDomainEntityDto<Platform.Model.Entities.Erm.OrderProcessingRequest> GetDto(long entityId)
        {
            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            var modelDto = _finder.Find(OrderProcessingRequestSpecifications.Find.ById(entityId))
                .Select(entity => new OrderProcessingRequestDomainEntityDto
                    {
                        Id = entity.Id,
                        ReplicationCode = entity.ReplicationCode,
                        State = (OrderProcessingRequestState)entity.State,
                        Title = entity.Title,
                        RequestType = (OrderProcessingRequestType)entity.RequestType,
                        BaseOrderRef = new EntityReference { Id = entity.BaseOrderId, Name = entity.BaseOrder.Number },
                        BeginDistributionDate = entity.BeginDistributionDate,
                        Description = entity.Description,
                        DueDate = entity.DueDate,
                        ReleaseCountPlan = entity.ReleaseCountPlan,
                        LegalPersonRef = new EntityReference { Id = entity.LegalPersonId, Name = entity.LegalPerson.LegalName },
                        LegalPersonProfileRef = new EntityReference { Id = entity.LegalPersonProfileId, Name = entity.LegalPersonProfile.Name },
                        FirmRef = new EntityReference { Id = entity.FirmId, Name = entity.Firm.Name },
                        RenewedOrderRef = new EntityReference { Id = entity.RenewedOrderId, Name = entity.RenewedOrder.Number },
                        SourceOrganizationUnitRef = new EntityReference { Id = entity.SourceOrganizationUnitId, Name = entity.SourceOrganizationUnit.Name },
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

            modelDto.DueDate = modelDto.DueDate.Add(timeOffset);

            return modelDto;
        }

        protected override IDomainEntityDto<Platform.Model.Entities.Erm.OrderProcessingRequest> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new OrderProcessingRequestDomainEntityDto();
        }
    }
}