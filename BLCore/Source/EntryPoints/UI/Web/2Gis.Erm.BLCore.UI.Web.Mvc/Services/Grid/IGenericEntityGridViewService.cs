using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using NuClear.Security.API.UserContext.Profile;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid
{
    public interface IEntityGridViewService : IUIService
    {
        EntityViewSet GetGridViewSettings(IUserProfile userProfile);
        EntityViewSet SecureViewsToolbars(EntityViewSet gridViewSettings, long? parentEntityId, EntityName parentEntityName, string parentEntityState);
    }

    public interface IGenericEntityGridViewService<TEntity> : IEntityUIService<TEntity>, IEntityGridViewService where TEntity : class, IEntityKey
    {
    }
}