using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class AdsTemplatesAdsElementTemplateGridViewService : GenericEntityGridViewService<AdsTemplatesAdsElementTemplate>
    {
        private readonly IAdvertisementRepository _advertisementRepository;

        public AdsTemplatesAdsElementTemplateGridViewService(
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IAdvertisementRepository advertisementRepository)
            : base(
            configurationService,
            entityAccessService,
            functionalAccessService,
            userContext)
        {
            _advertisementRepository = advertisementRepository;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(
            EntityViewSet gridViewSettings, 
            long? parentEntityId, 
            EntityName parentEntityName, 
            string parentEntityState)
        {
            if (parentEntityName != EntityName.AdvertisementTemplate || !parentEntityId.HasValue)
            {
                return gridViewSettings;
            }

            var advertisementTemplate = _advertisementRepository.GetAdvertisementTemplate(parentEntityId.Value);
            if (advertisementTemplate.IsPublished)
            {
                var createButtons = gridViewSettings.DataViews.SelectMany(x => x.ToolbarItems.Where(y => y.Name == "Create")).ToArray();
                foreach (var createButton in createButtons)
                {
                    createButton.Disabled = true;
                }

                var deleteButtons = gridViewSettings.DataViews.SelectMany(x => x.ToolbarItems.Where(y => y.Name == "Delete")).ToArray();
                foreach (var deleteButton in deleteButtons)
                {
                    deleteButton.Disabled = true;
                }
            }

            return gridViewSettings;
        }
    }
}
