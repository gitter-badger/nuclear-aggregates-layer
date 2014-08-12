using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class QdsListDepartmentService : ListEntityDtoServiceBase<Department, DepartmentGridDoc>
    {
        private readonly FilterHelper<DepartmentGridDoc> _filterHelper;

        public QdsListDepartmentService(FilterHelper<DepartmentGridDoc> filterHelper)
        {
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            string excludeId;
            if (querySettings.TryGetExtendedProperty("excludeId", out excludeId))
            {
                _filterHelper.AddFilter(x => x.Not(n => n.Term(t => t.Id, excludeId)));
            }

            return _filterHelper.Search(querySettings);
        }
    }
}