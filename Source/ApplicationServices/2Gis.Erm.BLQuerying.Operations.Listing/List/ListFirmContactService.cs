using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using System.Collections.Generic;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmContactService : ListEntityDtoServiceBase<FirmContact, ListFirmContactDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly FilterHelper _filterHelper;

        public ListFirmContactService(
            IFinder finder,
            IUserContext userContext,
            IFirmRepository firmRepository, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _firmRepository = firmRepository;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListFirmContactDto> List(QuerySettings querySettings, out int count)
        {
            IQueryable<FirmContact> query;
            if (querySettings.ParentEntityName == EntityName.FirmAddress && querySettings.ParentEntityId != null)
            {
                query = _firmRepository.GetContacts(querySettings.ParentEntityId.Value).AsQueryable();
            }
            else
            {
                query = _finder.FindAll<FirmContact>();
            }

            var data = query
            .Select(x => new ListFirmContactDto
            {
                Id = x.Id,
                ContactTypeEnum = (FirmAddressContactType)x.ContactType,
                Contact = x.Contact,
                CardId = x.CardId,
                FirmAddressId = x.FirmAddressId,
                ContactType = null,
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x =>
            {
                x.ContactType = x.ContactTypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                return x;
            });

            return data;
        }
    }
}