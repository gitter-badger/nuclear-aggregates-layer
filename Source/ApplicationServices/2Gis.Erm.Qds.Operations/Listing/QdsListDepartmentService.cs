using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class QdsListDepartmentService : ListEntityDtoServiceBase<Department, DepartmentGridDoc>
    {
        private readonly FilterHelper _filterHelper;

        public QdsListDepartmentService(FilterHelper filterHelper)
        {
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            return _filterHelper.Search<DepartmentGridDoc>(querySettings);
        }
    }
}