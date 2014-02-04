using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListFirmContactService : IListGenericEntityDtoService<FirmContact, ListFirmContactDto>
    {
        private readonly IQuerySettingsProvider _querySettingsProvider;
        private readonly IFirmRepository _firmRepository;
        private readonly IUserContext _userContext;

        public ListFirmContactService(IQuerySettingsProvider querySettingsProvider, IFirmRepository firmRepository, IUserContext userContext)
        {
            _querySettingsProvider = querySettingsProvider;
            _firmRepository = firmRepository;
            _userContext = userContext;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;

            var entityName = typeof(FirmContact).AsEntityName();

            var filterManager = new ListFilterManager(searchListModel);
            var querySettings = _querySettingsProvider.GetQuerySettings(entityName, searchListModel);
            var data = _firmRepository.GetContacts(filterManager.ParentEntityId)
                               .AsQueryable()
                               .ApplyQuerySettings(querySettings, out count)
                               .Select(x => new { x.Id, x.ContactType, x.Contact })
                               .AsEnumerable()
                               .Select(
                                   x =>
                                   new ListFirmContactDto
                                       {
                                           Id = x.Id,
                                           ContactType = ((FirmAddressContactType)x.ContactType).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                           Contact = x.Contact
                                       })
                               .ToArray();

            return new EntityDtoListResult<FirmContact, ListFirmContactDto>
            {
                Data = data,
                RowCount = count,
                MainAttribute = querySettings.MainAttribute
            };
        }
    }
}