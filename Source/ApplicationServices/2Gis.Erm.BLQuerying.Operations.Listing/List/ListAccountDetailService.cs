using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAccountDetailService : ListEntityDtoServiceBase<AccountDetail, ListAccountDetailDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAccountDetailService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAccountDetailDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AccountDetail>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListAccountDetailDto
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        AmountCharged = x.OperationType.IsPlus ? x.Amount : 0.0m,
                        AmountWithdrawn = !x.OperationType.IsPlus ? x.Amount : 0.0m,
                        OperationType = x.OperationType.Name,
                        TransactionDate = x.TransactionDate,
                        Description = x.Description,
                        IsDeleted = x.IsDeleted
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}