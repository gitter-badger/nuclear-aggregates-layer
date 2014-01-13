using System.Collections.Generic;
using System.Linq;
using System.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListLockDetailService : ListEntityDtoServiceBase<LockDetail, ListLockDetailDto>
    {
        public ListLockDetailService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListLockDetailDto> GetListData(IQueryable<LockDetail> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new { x.Id, CreateDate = x.CreatedOn, x.Amount, x.Description, x.LockId })
                .AsEnumerable()
                .Select(x => new ListLockDetailDto
                                {
                                    Id = x.Id, 
                                    CreateDate = x.CreateDate, 
                                    Amount = x.Amount, 
                                    Description = HttpUtility.HtmlEncode(x.Description), 
                                    LockId = x.LockId
                                });
        }
    }
}