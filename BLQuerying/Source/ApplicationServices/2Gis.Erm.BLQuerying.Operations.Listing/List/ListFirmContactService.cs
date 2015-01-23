using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmContactService : ListEntityDtoServiceBase<FirmContact, ListFirmContactDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IFirmReadModel _firmReadModel;

        public ListFirmContactService(
            IFinder finder,
            FilterHelper filterHelper,
            IFirmReadModel firmReadModel)
        {
            _finder = finder;
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
                query = _finder.FindAll<FirmContact>();
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