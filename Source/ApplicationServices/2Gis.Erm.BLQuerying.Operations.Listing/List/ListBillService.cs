using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBillService : ListEntityDtoServiceBase<Bill, ListBillDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBillService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Bill>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListBillDto
                {
                    Id = x.Id,
                    BillNumber = x.BillNumber,
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
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.BeginDistributionDate = new DateTime(x.BeginDistributionDate.Ticks, DateTimeKind.Local);
                    x.EndDistributionDate = new DateTime(x.EndDistributionDate.Ticks, DateTimeKind.Local);
                    x.CreatedOn = new DateTime(x.CreatedOn.Ticks, DateTimeKind.Local);

                    return x;
                });
        }
    }
}