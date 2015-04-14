using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ActionHistory
{
    public class ActionsHistoryService : IActionsHistoryService
    {
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IOperationsMetadataProvider _metadataProvider;
        private readonly IUserContext _userContext;

        public ActionsHistoryService(IFinder finder,
                                     ISecurityServiceUserIdentifier userIdentifierService,
                                     IOperationsMetadataProvider metadataProvider,
                                     IUserContext userContext)
        {
            _finder = finder;
            _userIdentifierService = userIdentifierService;
            _metadataProvider = metadataProvider;
            _userContext = userContext;
        }

        public ActionsHistoryDto GetActionHistory(EntityName entityName, long entityId)
        {
            if (!_metadataProvider.IsSupported<ActionHistoryIdentity>(entityName))
            {
                throw new InvalidOperationException("Can't get metadata for operation of type " + typeof(IActionsHistoryService) + " and entity type " +
                                                    entityName);
            }

            var userCultureInfo = _userContext.Profile.UserLocaleInfo.UserCultureInfo;
            var metadata = _metadataProvider.GetOperationMetadata<ActionHistoryMetadata, ActionHistoryIdentity>(entityName);
            var actionsInfo = _finder.Find<ActionsHistory>(x => x.EntityType == (int)entityName && x.EntityId == entityId)
                                     .OrderByDescending(x => x.Id)
                                     .Select(item => new
                                     {
                                         Item = new
                                         {
                                             item.Id,
                                             ActionType = (ActionType)item.ActionType,
                                             item.CreatedBy,
                                             item.CreatedOn
                                         },
                                         Details = item.ActionsHistoryDetails
                                                       .Where(detail => metadata.Properties.Contains(detail.PropertyName))
                                                       .Select(detail => new
                                                       {
                                                           detail.Id,
                                                           detail.ActionsHistoryId,
                                                           detail.PropertyName,
                                                           detail.OriginalValue,
                                                           detail.ModifiedValue
                                                       })
                                     })
                                     .ToArray();

            var actionHistoryData = actionsInfo
                .Select(x => new ActionsHistoryDto.ActionsHistoryItemDto
                {
                    Id = x.Item.Id,
                    ActionType = x.Item.ActionType.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    CreatedBy = _userIdentifierService.GetUserInfo(x.Item.CreatedBy).DisplayName,
                    CreatedOn = x.Item.CreatedOn
                })
                .ToArray();

            var actionHistoryDetailsData = actionsInfo.SelectMany(x => x.Details,
                                                                  (x, y) => new ActionsHistoryDto.ActionsHistoryDetailDto
                                                                  {
                                                                      Id = y.Id,
                                                                      ActionsHistoryId = y.ActionsHistoryId,
                                                                      PropertyName = MetadataResources.ResourceManager.GetString(y.PropertyName, userCultureInfo),
                                                                      OriginalValue = ProcessValue(entityName, y.PropertyName, y.OriginalValue, userCultureInfo),
                                                                      ModifiedValue = ProcessValue(entityName, y.PropertyName, y.ModifiedValue, userCultureInfo)
                                                                  })
                                                      .ToArray();

            return new ActionsHistoryDto
            {
                ActionHistoryData = actionHistoryData,
                ActionHistoryDetailsData = actionHistoryDetailsData
            };
        }

        private string ProcessValue(EntityName entityName, string propertyName, string value, CultureInfo userCultureInfo)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            switch (propertyName)
            {
                case "OwnerCode":
                    return _userIdentifierService.GetUserInfo(long.Parse(value)).DisplayName;
                case "ModifiedBy":
                    return _userIdentifierService.GetUserInfo(long.Parse(value)).DisplayName;
                case "WorkflowStepId":
                    return EnumUtils.ParseEnum<OrderState>(value).ToStringLocalized(EnumResources.ResourceManager, userCultureInfo);
                case "DealStage":
                    return EnumUtils.ParseEnum<DealStage>(value).ToStringLocalized(EnumResources.ResourceManager, userCultureInfo);
                case "Status":
                    if (entityName.IsActivity())
                    {
                        ActivityStatus status;
                        return EnumUtils.TryParseEnum(value, out status) ? status.ToStringLocalized(EnumResources.ResourceManager, userCultureInfo) : value;
                    }

                    if (entityName == EntityName.Limit)
                    {
                        LimitStatus status;
                        return EnumUtils.TryParseEnum(value, out status) ? status.ToStringLocalized(EnumResources.ResourceManager, userCultureInfo) : value;
                    }

                    if (entityName == EntityName.AdvertisementElementStatus)
                    {
                        AdvertisementElementStatusValue status;
                        return EnumUtils.TryParseEnum(value, out status) ? status.ToStringLocalized(EnumResources.ResourceManager, userCultureInfo) : value;
                    }

                    break;
            }

            return value;
        }
    }
}