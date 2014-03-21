﻿using System.Collections.Generic;
using System.Linq;
using System.Web;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLockDetailService : ListEntityDtoServiceBase<LockDetail, ListLockDetailDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListLockDetailService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListLockDetailDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<LockDetail>();

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
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.Description = HttpUtility.HtmlEncode(x.Description);
                    return x;
                });
        }
    }
}