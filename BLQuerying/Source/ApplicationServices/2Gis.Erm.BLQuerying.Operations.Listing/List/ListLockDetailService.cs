﻿using System.Linq;
using System.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLockDetailService : ListEntityDtoServiceBase<LockDetail, ListLockDetailDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListLockDetailService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<LockDetail>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListLockDetailDto
                {
                    Id= x.Id,
                    CreateDate = x.CreatedOn,
                    Amount = x.Amount,
                    Description = x.Description,
                    LockId = x.LockId,
                    IsActive = x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLockDetailDto dto)
        {
            dto.Description = HttpUtility.HtmlEncode(dto.Description);
        }
    }
}