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
    public sealed class ListLockService : ListEntityDtoServiceBase<Lock, ListLockDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListLockService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListLockDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Lock>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
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
                    x.AccountId,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListLockDto
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
                    AccountId = x.AccountId,
                });
        }
    }
}