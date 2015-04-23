using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings,
                                                                     long? parentEntityId,
                                                                     EntityName parentEntityName,
                                                                     string parentEntityState)
        {
            if (parentEntityName == EntityName.Bargain)
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