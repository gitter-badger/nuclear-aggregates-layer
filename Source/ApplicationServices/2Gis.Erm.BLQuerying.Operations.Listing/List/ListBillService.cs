﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListBillService : ListEntityDtoServiceBase<Bill, ListBillDto>
    {
        public ListBillService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListBillDto> GetListData(IQueryable<Bill> query, QuerySettings querySettings, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
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
                                x.CreatedOn
                            })
                .AsEnumerable()
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
                                CreatedOn = new DateTime(x.CreatedOn.Ticks, DateTimeKind.Local)
                            });
        }
    }
}