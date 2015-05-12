using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OrderObtainer : IBusinessModelEntityObtainer<Order>, IAggregateReadModel<Order>
    {
        private readonly IFinder _finder;

        public OrderObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Order ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderDomainEntityDto)domainEntityDto;

            var order = _finder.Find(Specs.Find.ById<Order>(dto.Id)).SingleOrDefault() ??
                        new Order
                            {
                                IsActive = true,
                                LegalPersonProfileId = dto.LegalPersonProfileRef != null ? dto.LegalPersonProfileRef.Id : null,
                                HasDocumentsDebt = dto.HasDocumentsDebt
                            };

            order.Number = dto.Number;
            order.RegionalNumber = dto.RegionalNumber;
            order.FirmId = dto.FirmRef.Id.Value;
            order.SourceOrganizationUnitId = dto.SourceOrganizationUnitRef.Id.Value;
            order.DestOrganizationUnitId = dto.DestOrganizationUnitRef.Id.Value;
            order.BranchOfficeOrganizationUnitId = dto.BranchOfficeOrganizationUnitRef.Id;
            order.LegalPersonId = dto.LegalPersonRef.Id;
            order.LegalPersonProfileId = dto.LegalPersonProfileRef.Id;
            order.CurrencyId = dto.CurrencyRef.Id;
            order.DealId = dto.DealRef != null ? dto.DealRef.Id : null;
            order.DgppId = dto.DgppId;
            order.BeginDistributionDate = dto.BeginDistributionDate;
            order.SignupDate = dto.SignupDate;
            order.ReleaseCountPlan = dto.ReleaseCountPlan;
            order.ReleaseCountFact = dto.ReleaseCountFact;
            order.WorkflowStepId = dto.WorkflowStepId;
            order.IsTerminated = dto.IsTerminated;
            order.DiscountReasonEnum = dto.DiscountReasonEnum;
            order.DiscountComment = dto.DiscountComment;
            order.BargainId = dto.BargainRef.Id;
            order.Comment = dto.Comment;
            order.DiscountPercent = dto.DiscountPercent;
            order.DiscountSum = dto.DiscountSum;
            order.InspectorCode = dto.InspectorRef.Id;
            order.TerminationReason = dto.TerminationReason;
            order.OrderType = dto.OrderType;
            order.PlatformId = dto.PlatformRef.Id;
            order.PaymentMethod = dto.PaymentMethod;
            order.OwnerCode = dto.OwnerRef.Id.Value;
            order.Timestamp = dto.Timestamp;

            return order;
        }
    }
}