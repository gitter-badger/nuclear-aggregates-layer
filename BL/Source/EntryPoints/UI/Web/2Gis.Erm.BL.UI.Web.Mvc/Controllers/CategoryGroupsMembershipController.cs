using System.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
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
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Serialization;
using DoubleGis.Erm.Platform.Model.Entities;
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
        private readonly ICategoryService _categoryService;
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
                                                  ICategoryService categoryService,
                                                  ICategoryReadModel categoryReadModel,
                                                  IOrganizationUnitReadModel organizationUnitReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _configurationService = configurationService;
            _categoryService = categoryService;
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
            var cardSettings = _configurationService.GetCardSettings(EntityName.CategoryGroupMembership, UserContext.Profile.UserLocaleInfo.UserCultureInfo);
            cardSettings.CardLocalizedName = string.Format(BLResources.OrganizationUnitCategoryGroupsCardTitle, orgUnit.Name);

            var model = new CategoryGroupMembershipViewModel
                {
                    OrganizationUnitId = organizationUnitId,
                    ViewConfig =
                        {
                            EntityName = EntityName.CategoryGroupMembership, 
                            PType = EntityName.None, 
                            CardSettings = cardSettings.ToCardJson()
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
            _categoryService.SetCategoryGroupMembership(organizationUnitId, deserializedData);
            return new JsonNetResult(new { categoryGroupsMembership = deserializedData, success = true });
        }
    }
}
