using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class LimitGridViewService : GenericEntityGridViewService<Limit>
    {
        public LimitGridViewService(
            IUIConfigurationService configurationService, 
            ISecurityServiceEntityAccessInternal entityAccessService, 
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            if (!parentEntityName.Equals(EntityType.Instance.Account()))
            {
                var itemsToDelete = new[] { "Create", "Delete", "Splitter" };

                foreach (var dataView in gridViewSettings.DataViews)
                {
                    dataView.ToolbarItems = dataView.ToolbarItems.Where(x => !itemsToDelete.Contains(x.Name)).ToArray();
                }
            }

            return gridViewSettings;
        }
    }
}