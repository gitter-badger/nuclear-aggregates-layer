using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOperationTypeService : ListEntityDtoServiceBase<OperationType, ListOperationTypeDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListOperationTypeService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<OperationType>();

            return query
                .Select(x => new ListOperationTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    OperationTypeName = x.IsPlus ? BLResources.Charge : BLResources.Withdrawal,
                    SyncCode1C = x.SyncCode1C,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}