using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserBranchOfficeService : ListEntityDtoServiceBase<UserBranchOffice, ListUserBranchOfficeDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListUserBranchOfficeService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<UserBranchOffice>();

            var data = query
                .Select(x => new ListUserBranchOfficeDto
                                 {
                                     Id = x.Id,
                                     UserId = x.UserId,
                                     BranchOfficeId = x.BranchOfficeId,
                                     UserName = x.User.DisplayName,
                                     IsDeleted = x.IsDeleted
                                 })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}