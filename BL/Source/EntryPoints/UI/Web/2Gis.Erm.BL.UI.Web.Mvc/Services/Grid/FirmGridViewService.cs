using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class FirmGridViewService : GenericEntityGridViewService<Firm>
    {
        public FirmGridViewService(
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
            if (parentEntityName != EntityName.Client)
            {
                var detachButtons = gridViewSettings.DataViews.SelectMany(x => x.ToolbarItems.Where(y => y.Name == "DetachFirm")).ToArray();
                foreach (var detachButton in detachButtons)
                {
                    detachButton.Disabled = true;
                }
            }

            return gridViewSettings;
        }
    }
}
