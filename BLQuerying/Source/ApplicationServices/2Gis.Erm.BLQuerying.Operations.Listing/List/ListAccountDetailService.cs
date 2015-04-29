using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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

        public ListAccountDetailService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<AccountDetail>();

            return query
                .Select(x => new ListAccountDetailDto
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        AmountCharged = x.OperationType.IsPlus ? x.Amount : 0.0m,
                        AmountWithdrawn = !x.OperationType.IsPlus ? x.Amount : 0.0m,
                        OperationType = x.OperationType.Name,
                        TransactionDate = x.TransactionDate,
                        Description = x.Description,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}