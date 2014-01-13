using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListLocalMessageService : ListEntityDtoServiceBase<LocalMessage, ListLocalMessageDto>
    {
        public ListLocalMessageService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListLocalMessageDto> GetListData(IQueryable<LocalMessage> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
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
                .AsEnumerable()
                .Select(x =>
                        new ListLocalMessageDto
                            {
                                Id = x.Id,
                                IntegrationType =
                                    x.IntegrationTypeImport.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo)
                                    ?? x.IntegrationTypeExport.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                CreatedOn = x.CreatedOn,
                                ModifiedOn = x.ModifiedOn,
                                Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                SenderSystem = x.SenderSystem.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                ReceiverSystem = x.ReceiverSystem.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo)
                            });
        }
    }
}