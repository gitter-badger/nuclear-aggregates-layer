using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOperationTypeService : ListEntityDtoServiceBase<OperationType, ListOperationTypeDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListOperationTypeService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<OperationType>();

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