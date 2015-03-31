using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class OrderFileViewService : GenericEntityGridViewService<OrderFile>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly IUserContext _userContext;

        public OrderFileViewService(IUIConfigurationService configurationService,
                                    ISecurityServiceEntityAccessInternal entityAccessService,
                                    ISecurityServiceFunctionalAccess functionalAccessService,
                                    IUserContext userContext,
                                    IOrderReadModel orderReadModel)
            : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
            _orderReadModel = orderReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings,
                                                                     long? parentEntityId,
                                                                     EntityName parentEntityName,
                                                                     string parentEntityState)
        {
            if (parentEntityName == EntityName.Order && parentEntityId.HasValue)
            {
                var order = _orderReadModel.GetOrderSecure(parentEntityId.Value);
                var hasUserRightsToEditOrder = _entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                                                    EntityName.Order,
                                                                                    _userContext.Identity.Code,
                                                                                    order.Id,
                                                                                    order.OwnerCode,
                                                                                    order.OwnerCode);

                if (!hasUserRightsToEditOrder)
                {
                    var buttonsToDisable =
                        gridViewSettings.DataViews.SelectMany(x => x.ToolbarItems.Where(y => y.Name == "Create" || y.Name == "Delete")).ToArray();

                    foreach (var button in buttonsToDisable)
                    {
                        button.Disabled = true;
                    }
                }
            }

            return gridViewSettings;
        }
    }
}

