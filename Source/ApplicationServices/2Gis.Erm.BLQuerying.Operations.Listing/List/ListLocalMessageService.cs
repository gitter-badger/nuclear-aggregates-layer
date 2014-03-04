using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLocalMessageService : ListEntityDtoServiceBase<LocalMessage, ListLocalMessageDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListLocalMessageService(
            IQuerySettingsProvider querySettingsProvider,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListLocalMessageDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<LocalMessage>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                                 {
                                     x.Id,
                                     IntegrationTypeImport = (IntegrationTypeImport)x.MessageType.IntegrationType,
                                     IntegrationTypeExport = (IntegrationTypeExport)x.MessageType.IntegrationType,
                                     x.OrganizationUnitId,
                                     OrganizationUnitName = x.OrganizationUnit.Name,
                                     x.CreatedOn,
                                     x.ModifiedOn,
                                     Status = (LocalMessageStatus)x.Status,
                                     SenderSystem = (IntegrationSystem)x.MessageType.SenderSystem,
                                     ReceiverSystem = (IntegrationSystem)x.MessageType.ReceiverSystem,
                                 })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new ListLocalMessageDto
                            {
                                Id = x.Id,
                                IntegrationType =
                                    x.IntegrationTypeImport.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
                                    ?? x.IntegrationTypeExport.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                CreatedOn = x.CreatedOn,
                                ModifiedOn = x.ModifiedOn,
                                Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                SenderSystem = x.SenderSystem.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                ReceiverSystem = x.ReceiverSystem.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
                            });
        }
    }
}