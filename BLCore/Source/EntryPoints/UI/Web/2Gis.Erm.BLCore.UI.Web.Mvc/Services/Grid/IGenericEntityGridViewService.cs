using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext.Profile;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid
{
    public interface IEntityGridViewService : IUIService
    {
        EntityViewSet GetGridViewSettings(IUserProfile userProfile);
        EntityViewSet SecureViewsToolbars(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState);
    }

    public interface IGenericEntityGridViewService<TEntity> : IEntityUIService<TEntity>, IEntityGridViewService where TEntity : class, IEntityKey
    {
    }
}