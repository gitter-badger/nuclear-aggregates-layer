using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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

        public ListLockService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Lock>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListLockDto
                {
                    OrderNumber = x.Order.Number,
                    CreateDate = x.CreatedOn,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate,
                    PlannedAmount = x.PlannedAmount,
                    Balance = x.Balance,
                    OwnerCode = x.OwnerCode,
                    Id = x.Id,
                    OrderId = x.OrderId,
                    AccountId = x.AccountId,
                    IsActive = x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLockDto dto)
        {
            dto.PeriodStartDate = new DateTime(dto.PeriodStartDate.Ticks, DateTimeKind.Local);
            dto.PeriodEndDate = new DateTime(dto.PeriodEndDate.Ticks, DateTimeKind.Local);
        }
    }
}