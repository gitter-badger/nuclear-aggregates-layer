using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
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
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LocalMessage>();

            return query
                .Select(x => new ListLocalMessageDto
                {
                    Id = x.Id,
                    IntegrationTypeImport = (IntegrationTypeImport)x.MessageType.IntegrationType,
                    IntegrationTypeExport = (IntegrationTypeExport)x.MessageType.IntegrationType,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    StatusEnum = (LocalMessageStatus)x.Status,
                    SenderSystemEnum = (IntegrationSystem)x.MessageType.SenderSystem,
                    ReceiverSystemEnum = (IntegrationSystem)x.MessageType.ReceiverSystem,
                    IntegrationType = null,
                    Status = null,
                    ReceiverSystem = null,
                    SenderSystem = null,
                })
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.IntegrationType =
                        x.IntegrationTypeImport.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
                        ??
                        x.IntegrationTypeExport.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

                    x.Status = x.StatusEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.SenderSystem = x.SenderSystemEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.ReceiverSystem = x.ReceiverSystemEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

                    return x;
                });
        }
    }
}