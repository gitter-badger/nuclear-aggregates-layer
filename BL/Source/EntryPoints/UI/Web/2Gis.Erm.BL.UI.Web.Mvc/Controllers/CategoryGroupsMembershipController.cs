using System.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Serialization;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class CategoryGroupsMembershipController : ControllerBase
    {
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly ICategoryReadModel _categoryReadModel;
        private readonly IUIConfigurationService _configurationService;
        private readonly IChangeCategoryGroupService _changeCategoryGroupService;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        public CategoryGroupsMembershipController(IMsCrmSettings msCrmSettings,
                                                  IAPIOperationsServiceSettings operationsServiceSettings,
                                                  IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                                  IAPIIdentityServiceSettings identityServiceSettings,
                                                  IUserContext userContext,
                                                  ITracer tracer,
                                                  IGetBaseCurrencyService getBaseCurrencyService,
                                                  ISecurityServiceEntityAccess securityServiceEntityAccess,
                                                  IUIConfigurationService configurationService,
                                                  IChangeCategoryGroupService changeCategoryGroupService,
                                                  ICategoryReadModel categoryReadModel,
                                                  IOrganizationUnitReadModel organizationUnitReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _configurationService = configurationService;
            _changeCategoryGroupService = changeCategoryGroupService;
            _categoryReadModel = categoryReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
        }

        [HttpGet]
        public ActionResult Manage(long organizationUnitId)
        {
            var hasClientPrivileges = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                   EntityName.OrganizationUnit,
                                                                                   UserContext.Identity.Code,
                                                                                   organizationUnitId,
                                                                                   -1, // TODO {all}: Сделать с этим что-то порядочное
                                                                                   null);
            if (!hasClientPrivileges)
            {
                throw new SecurityException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            var orgUnit = _organizationUnitReadModel.GetOrganizationUnit(organizationUnitId);
            var cardSettings = GetCategoryGroupMembershipSettings();
            cardSettings.Title = string.Format(BLResources.OrganizationUnitCategoryGroupsCardTitle, orgUnit.Name);

            var model = new CategoryGroupMembershipViewModel
            {
                OrganizationUnitId = organizationUnitId,
                ViewConfig =
                {
                    EntityName = EntityName.CategoryGroupMembership,
                    PType = EntityName.None,
                    CardSettings = cardSettings
                }
            };

            return View(model);
        }

        [HttpGet]
        public JsonNetResult CategoryGroups()
        {
            var allCategoryGroups = _categoryReadModel.GetCategoryGroups();
            return new JsonNetResult(allCategoryGroups);
        }

        [HttpGet]
        public JsonNetResult CategoryGroupsMembership(long organizationUnitId)
        {
            var categoryDtos = _categoryReadModel.GetCategoryGroupMembership(organizationUnitId);
            return new JsonNetResult(new { categoryGroupsMembership = categoryDtos, success = true });
        }

        [HttpPost]
        public JsonNetResult CategoryGroupsMembership(long organizationUnitId, string categoryGroupsMembership)
        {
            var serializerSettings = new JsonSerializerSettings { Converters = { new Int64ToStringConverter() } };
            var deserializedData = JsonConvert.DeserializeObject<CategoryGroupMembershipDto[]>(categoryGroupsMembership, serializerSettings);
            _changeCategoryGroupService.SetCategoryGroupMembership(deserializedData);
            return new JsonNetResult(new { categoryGroupsMembership = deserializedData, success = true });
        }

        private CardStructure GetCategoryGroupMembershipSettings()
        {
            return new CardStructure
                       {
                           Icon = "en_ico_lrg_Category.gif",
                           EntityName = EntityName.CategoryGroupMembership.ToString(),
                           EntityLocalizedName = ErmConfigLocalization.EnCategoryGroups,
                           CardRelatedItems = new CardRelatedItemsGroupStructure[0],
                           CardToolbar = new[]
                                             {
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Save",
                                                         LocalizedName = ErmConfigLocalization.ControlSave,
                                                         ControlType = ControlType.ImageButton.ToString(),
                                                         Action = "scope.Save",
                                                         Icon = "Save.gif",

                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                         SecurityPrivelege = 34
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Splitter",
                                                         LocalizedName = ErmConfigLocalization.ControlSplitter,
                                                         ControlType = ControlType.Splitter.ToString(),
                                                         
                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "SaveAndClose",
                                                         LocalizedName = ErmConfigLocalization.ControlSaveAndClose,
                                                         ControlType = ControlType.TextImageButton.ToString(),
                                                         Action = "scope.SaveAndClose",
                                                         Icon = "SaveAndClose.gif",

                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                         SecurityPrivelege = 34
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Splitter",
                                                         LocalizedName = ErmConfigLocalization.ControlSplitter,
                                                         ControlType = ControlType.Splitter.ToString(),
                                                         
                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Refresh",
                                                         LocalizedName = ErmConfigLocalization.ControlRefresh,
                                                         ControlType = ControlType.TextImageButton.ToString(),
                                                         Action = "scope.refresh",
                                                         Icon = "Refresh.gif",
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Splitter",
                                                         LocalizedName = ErmConfigLocalization.ControlSplitter,
                                                         ControlType = ControlType.Splitter.ToString(),
                                                         
                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "ViewCategoryGroups",
                                                         LocalizedName = ErmConfigLocalization.ControlViewCategoryGroups,
                                                         ControlType = ControlType.TextImageButton.ToString(),
                                                         Action = "scope.ViewCategoryGroups",
                                                         Icon = "en_ico_16_Category.gif",
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Splitter",
                                                         LocalizedName = ErmConfigLocalization.ControlSplitter,
                                                         ControlType = ControlType.Splitter.ToString(),
                                                         
                                                         // Никто на это не смотрит
                                                         LockOnInactive = true,
                                                     },
                                                 new ToolbarElementStructure
                                                     {
                                                         Name = "Close",
                                                         LocalizedName = ErmConfigLocalization.ControlClose,
                                                         ControlType = ControlType.TextImageButton.ToString(),
                                                         Action = "scope.Close",
                                                         Icon = "Close.gif",
                                                     },
                                             }
                       };
        }

    }
}
