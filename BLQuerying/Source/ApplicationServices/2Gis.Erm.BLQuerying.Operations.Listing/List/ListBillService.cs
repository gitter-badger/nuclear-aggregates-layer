using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBillService : ListEntityDtoServiceBase<Bill, ListBillDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListBillService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Bill>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListBillDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    OrderNumber = x.Order.Number,
                    FirmId = x.Order.FirmId,
                    FirmName = x.Order.Firm.Name,
                    ClientId = x.Order.Firm.ClientId,
                    ClientName = x.Order.Firm.Client.Name,
                    BeginDistributionDate = x.BeginDistributionDate,
                    EndDistributionDate = x.EndDistributionDate,
                    PayablePlan = x.PayablePlan,
                    PaymentDatePlan = x.PaymentDatePlan,
                    CreatedOn = x.CreatedOn,
                    OrderId = x.OrderId,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListBillDto dto)
        {
            dto.BeginDistributionDate = new DateTime(dto.BeginDistributionDate.Ticks, DateTimeKind.Local);
            dto.EndDistributionDate = new DateTime(dto.EndDistributionDate.Ticks, DateTimeKind.Local);
            dto.CreatedOn = new DateTime(dto.CreatedOn.Ticks, DateTimeKind.Local);
        }
    }
}