using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListClientLinkService : ListEntityDtoServiceBase<ClientLink, ListClientLinkDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListClientLinkService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<ClientLink>();

            var myChildLinks = querySettings
                .CreateForExtendedProperty<ClientLink, bool>("ClientLinks", info => e => e.MasterClientId == querySettings.ParentEntityId);

            var myMasterLinks = querySettings
                .CreateForExtendedProperty<ClientLink, bool>("ClientLinksMaster", info => e => e.ChildClientId == querySettings.ParentEntityId);

            var myDeletedLinks = querySettings
                .CreateForExtendedProperty<ClientLink, bool>("ClientLinksDeleted", info => e => e.ChildClientId == querySettings.ParentEntityId || e.MasterClientId == querySettings.ParentEntityId);

            return query.Filter(_filterHelper, myChildLinks, myMasterLinks, myDeletedLinks)
                .Select(x => new ListClientLinkDto
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    ChildClientId = x.ChildClientId,
                    ChildClientName = x.ChildClient.Name,
                    CreatedBy = x.CreatedBy,
                    IsDeleted = x.IsDeleted,
                    MasterClientId = x.MasterClientId,
                    MasterClientName = x.MasterClient.Name,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedOn = x.ModifiedOn
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}