using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDenialReasonService : ListEntityDtoServiceBase<DenialReason, ListDenialReasonDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListDenialReasonService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<DenialReason>();

            return query
                .Select(x => new ListDenialReasonDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedOn = x.CreatedOn,
                        IsActive = x.IsActive,
                        Type = x.Type.ToStringLocalizedExpression(),
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}