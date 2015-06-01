using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmContactService : ListEntityDtoServiceBase<FirmContact, ListFirmContactDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;
        private readonly IFirmReadModel _firmReadModel;

        public ListFirmContactService(
            IQuery query,
            FilterHelper filterHelper,
            IFirmReadModel firmReadModel)
        {
            _query = query;
            _filterHelper = filterHelper;
            _firmReadModel = firmReadModel;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            IQueryable<FirmContact> query;
            if (querySettings.ParentEntityName.Equals(EntityType.Instance.FirmAddress()) && querySettings.ParentEntityId != null)
            {
                query = _firmReadModel.GetContacts(querySettings.ParentEntityId.Value).AsQueryable();
            }
            else
            {
                query = _query.For<FirmContact>();
            }

            var data = query
            .Select(x => new ListFirmContactDto
            {
                Id = x.Id,
                Contact = x.Contact,
                CardId = x.CardId,
                FirmAddressId = x.FirmAddressId,
                ContactType = x.ContactType.ToStringLocalizedExpression(),
            })
            .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}