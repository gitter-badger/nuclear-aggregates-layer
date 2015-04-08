﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class OrderGridViewService : GenericEntityGridViewService<Order>
    {
        public OrderGridViewService(
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
            : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            if (parentEntityName.Equals(EntityType.Instance.Bargain()))
            {
                foreach (var dataView in gridViewSettings.DataViews)
                {
                    dataView.ToolbarItems = Enumerable.Empty<ToolbarElementStructure>();
                }
            }

            return gridViewSettings;
        }
    }
}