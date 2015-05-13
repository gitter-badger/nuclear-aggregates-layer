using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Generic.Modify.DomainEntityObtainers
{
    public class OrderProcessingRequestObtainer : ISimplifiedModelEntityObtainer<OrderProcessingRequest>
    {
        private readonly IFinder _finder;

        public OrderProcessingRequestObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public OrderProcessingRequest ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderProcessingRequestDomainEntityDto)domainEntityDto;

            var result = _finder.FindOne(Specs.Find.ById<OrderProcessingRequest>(dto.Id))
                ?? new OrderProcessingRequest { IsActive = true };

            result.BaseOrderId = dto.BaseOrderRef.Id;
            result.RenewedOrderId = dto.RenewedOrderRef.Id;
            result.BeginDistributionDate = dto.BeginDistributionDate;
            result.Description = dto.Description;
            result.DueDate = dto.DueDate;
            result.FirmId = dto.FirmRef.Id.Value;
            result.LegalPersonId = dto.LegalPersonRef.Id.Value;
            result.LegalPersonProfileId = dto.LegalPersonProfileRef.Id.Value;
            result.OwnerCode = dto.OwnerRef.Id.Value;
            result.ReplicationCode = dto.ReplicationCode;
            result.SourceOrganizationUnitId = dto.SourceOrganizationUnitRef.Id.Value;
            result.State = dto.State;
            result.Title = dto.Title;
            result.RequestType = dto.RequestType;
            result.Timestamp = dto.Timestamp;

            return result;
        }
    }
}