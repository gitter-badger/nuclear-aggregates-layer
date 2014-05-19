﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class OrderFileViewService : GenericEntityGridViewService<Order>
    {
        private readonly IOrderReadModel _orderReadModel;

        public OrderFileViewService(IUIConfigurationService configurationService,
                                    ISecurityServiceEntityAccessInternal entityAccessService,
                                    ISecurityServiceFunctionalAccess functionalAccessService,
                                    IUserContext userContext,
                                    IOrderReadModel orderReadModel)
            : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
            _orderReadModel = orderReadModel;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings,
                                                                     long? parentEntityId,
                                                                     EntityName parentEntityName,
                                                                     string parentEntityState)
        {
            if (parentEntityName == EntityName.Order && parentEntityId.HasValue)
            {
                var order = _orderReadModel.GetOrder(parentEntityId.Value);
                if (!order.IsActive || order.IsDeleted || order.IsTerminated)
                {
                    var createButtons =
                        gridViewSettings.DataViews.SelectMany(x => x.ToolbarItems.Where(y => y.Name == "Create")).ToArray();

                    foreach (var createButton in createButtons)
                    {
                        createButton.Disabled = true;
                    }
                }
            }

            return gridViewSettings;
        }
    }
}



