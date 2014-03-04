using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
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
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListBillDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Bill>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x =>
                        new
                            {
                                x.Id,
                                x.BillNumber,
                                OrderNumber = x.Order.Number,
                                x.Order.FirmId,
                                FirmName = x.Order.Firm.Name,
                                x.Order.Firm.ClientId,
                                ClientName = x.Order.Firm.Client.Name,
                                x.BeginDistributionDate,
                                x.EndDistributionDate,
                                x.PayablePlan,
                                x.PaymentDatePlan,
                                x.CreatedOn,
                                x.OrderId,
                            })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new ListBillDto
                            {
                                Id = x.Id,
                                BillNumber = x.BillNumber,
                                OrderNumber = x.OrderNumber,
                                FirmId = x.FirmId,
                                FirmName = x.FirmName,
                                ClientId = x.ClientId,
                                ClientName = x.ClientName,
                                BeginDistributionDate = new DateTime(x.BeginDistributionDate.Ticks, DateTimeKind.Local),
                                EndDistributionDate = new DateTime(x.EndDistributionDate.Ticks, DateTimeKind.Local),
                                PayablePlan = x.PayablePlan,
                                PaymentDatePlan = x.PaymentDatePlan,
                                CreatedOn = new DateTime(x.CreatedOn.Ticks, DateTimeKind.Local),
                                OrderId = x.OrderId,
                            });
        }
    }
}