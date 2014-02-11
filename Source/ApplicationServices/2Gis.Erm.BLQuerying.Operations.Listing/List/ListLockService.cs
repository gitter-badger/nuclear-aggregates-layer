using System;
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
    public class ListLockService : ListEntityDtoServiceBase<Lock, ListLockDto>
    {
        public ListLockService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListLockDto> GetListData(IQueryable<Lock> query, QuerySettings querySettings, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new
                            {
                                OrderNumber = x.Order.Number,
                                CreateDate = x.CreatedOn,
                                x.PeriodStartDate,
                                x.PeriodEndDate,
                                x.PlannedAmount,
                                x.Balance,
                                x.OwnerCode,
                                x.Id,
                                x.OrderId,
                                x.AccountId
                            })
                .AsEnumerable()
                .Select(x =>
                        new ListLockDto
                            {
                                OrderNumber = x.OrderNumber,
                                CreateDate = x.CreateDate,
                                PeriodStartDate = new DateTime(x.PeriodStartDate.Ticks, DateTimeKind.Local),
                                PeriodEndDate = new DateTime(x.PeriodEndDate.Ticks, DateTimeKind.Local),
                                PlannedAmount = x.PlannedAmount,
                                Balance = x.Balance,
                                OwnerCode = x.OwnerCode,
                                Id = x.Id,
                                OrderId = x.OrderId,
                                AccountId = x.AccountId
                            });
        }
    }
}