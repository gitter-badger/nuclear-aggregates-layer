using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAccountDetailService : ListEntityDtoServiceBase<AccountDetail, ListAccountDetailDto>
    {
        public ListAccountDetailService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAccountDetailDto> GetListData(IQueryable<AccountDetail> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Select(x => new
                    {
                        x.Id,
                        x.AccountId,
                        x.Amount,
                        x.OperationType.IsPlus,
                        OperationType = x.OperationType.Name,
                        x.TransactionDate,
                        x.Description,
                        x.IsDeleted
                    })
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new ListAccountDetailDto
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        OperationType = x.OperationType,
                        AmountCharged = x.IsPlus ? x.Amount : 0.0m,
                        AmountWithdrawn = !x.IsPlus ? x.Amount : 0.0m,
                        TransactionDate = x.TransactionDate,
                        Description = x.Description,
                        IsDeleted = x.IsDeleted
                    });
        }
    }
}