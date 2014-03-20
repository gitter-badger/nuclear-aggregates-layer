using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<ListOperationTypeDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<OperationType>();

            var syncCodeFilter = querySettings.CreateForExtendedProperty<OperationType, string>("excludeSyncCode", code => x => x.SyncCode1C != code);

            return query
                .Filter(_filterHelper, syncCodeFilter)
                .Select(x => new ListOperationTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    OperationTypeName = x.IsPlus ? BLResources.Charge : BLResources.Withdrawal,
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}