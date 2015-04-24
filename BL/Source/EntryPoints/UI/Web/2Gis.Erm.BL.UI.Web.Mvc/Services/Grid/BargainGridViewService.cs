using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class BargainGridViewService : GenericEntityGridViewService<Bargain>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;

        public BargainGridViewService(
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
            : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
            _userContext = userContext;
            _securityServiceFunctionalAccess = functionalAccessService;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            if (!parentEntityName.Equals(EntityType.Instance.None()) ||
                !_securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CloseBargainOperationalPeriod,
                                                                                _userContext.Identity.Code))
            {
                foreach (var dataView in gridViewSettings.DataViews)
                {
                    dataView.ToolbarItems = dataView.ToolbarItems.Where(y => y.Name != "CloseBargains");
                }
            }

            return gridViewSettings;
        }
    }
}