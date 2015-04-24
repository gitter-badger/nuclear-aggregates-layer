using System;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;


using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Profile;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid
{
    public class GenericEntityGridViewService<TEntity> : IGenericEntityGridViewService<TEntity> where TEntity : class, IEntityKey
    {
        private readonly IUIConfigurationService _configurationService;
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public GenericEntityGridViewService(
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _configurationService = configurationService;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public EntityViewSet GetGridViewSettings(IUserProfile userProfile)
        {
            var gridViewSettings = _configurationService.GetGridSettings(typeof(TEntity).AsEntityName(), userProfile.UserLocaleInfo.UserCultureInfo);
            return gridViewSettings.ToEntityViewSet();
        }

        public EntityViewSet SecureViewsToolbars(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            var readOnly = parentEntityState == "Inactive";

            var entityAccessTypes = _entityAccessService.GetCommonEntityAccessForMetadata(typeof(TEntity).AsEntityName(), _userContext.Identity.Code);

            foreach (var dataViewJson in gridViewSettings.DataViews)
            {
                foreach (var toolbarItem in dataViewJson.ToolbarItems)
                {
                    if (toolbarItem.LockOnNew || (readOnly && toolbarItem.LockOnInactive))
                    {
                        toolbarItem.Disabled = true;
                        continue;
                    }

                    var privilegeMask = toolbarItem.SecurityPrivelege;
                    if (privilegeMask == null || privilegeMask.Value == 0)
                    {
                        toolbarItem.Disabled = false;
                        continue;
                    }

                    if (!_entityAccessService.IsSecureEntity(typeof(TEntity).AsEntityName()))
                    {
                        continue;
                    }

                    if (Enum.IsDefined(typeof(FunctionalPrivilegeName), privilegeMask.Value))
                    {
                        toolbarItem.Disabled = !_functionalAccessService.HasFunctionalPrivilegeGranted((FunctionalPrivilegeName)privilegeMask.Value, _userContext.Identity.Code);
                    }
                    else
                    {
                        toolbarItem.Disabled = !entityAccessTypes.HasFlag((EntityAccessTypes)privilegeMask.Value);
                    }
                }
            }

            gridViewSettings = SecureViewsToolbarsInternal(gridViewSettings, parentEntityId, parentEntityName, parentEntityState);
            return gridViewSettings;
        }

        protected virtual EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            return gridViewSettings;
        }
    }
}