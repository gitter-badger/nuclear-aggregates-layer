﻿using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Serialization;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class CategoryGroupsMembershipController : ControllerBase
    {
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserRepository _userRepository;
        private readonly IUIConfigurationService _configurationService;
        private readonly ICategoryService _categoryService;

        public CategoryGroupsMembershipController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            ICategoryService categoryService, 
            ISecurityServiceEntityAccess securityServiceEntityAccess, 
            IUserRepository userRepository, 
            IUIConfigurationService configurationService, 
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
            _categoryService = categoryService;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userRepository = userRepository;
            _configurationService = configurationService;
        }

        [HttpGet]
        public ActionResult Manage(long organizationUnitId)
        {
            var orgUnit = _userRepository.GetOrganizationUnit(organizationUnitId);
            if (orgUnit == null)
            {
                throw new NotificationException(BLResources.EntityNotFound);
            }

            var hasClientPrivileges = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                   EntityName.OrganizationUnit,
                                                                                   UserContext.Identity.Code,
                                                                                   organizationUnitId,
                                                                                   -1, //TODO: Сделать с этим что-то порядочное
                                                                                   null);
            if (!hasClientPrivileges)
            {
                throw new SecurityException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            var model = new CategoryGroupMembershipViewModel
                {
                    OrganizationUnitId = organizationUnitId,
                    ViewConfig =
                        {
                            EntityName = EntityName.CategoryGroupMembership, 
                            PType = EntityName.None
                        }
                };

            var cardSettings = _configurationService.GetCardSettings(EntityName.CategoryGroupMembership, UserContext.Profile.UserLocaleInfo.UserCultureInfo);
            cardSettings.CardLocalizedName = string.Format(BLResources.OrganizationUnitCategoryGroupsCardTitle, orgUnit.Name);

            model.ViewConfig.CardSettings = cardSettings.ToCardJson();

            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(CategoryGroupMembershipViewModel model)
        {
            model.ViewConfig.EntityName = EntityName.CategoryGroupMembership;
            model.ViewConfig.PType = EntityName.None;

            var cardSettings = _configurationService.GetCardSettings(EntityName.CategoryGroupMembership, UserContext.Profile.UserLocaleInfo.UserCultureInfo);
            model.ViewConfig.CardSettings = cardSettings.ToCardJson();

            return new JsonNetResult(model);
        }

        [HttpGet]
        public JsonNetResult GetCategoryGroupsMembership(long organizationUnitId)
        {
            IEnumerable<CategoryGroupMembershipDto> categoriesDtos = _userRepository.GetCategoryGroupMembership(organizationUnitId);
            var allCategoryGroups = _userRepository.GetCategoryGroups();

            var categoryGroupsMembership = categoriesDtos.Select(x => new
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryGroupId = x.CategoryGroupId,
                CategoryLevel = x.CategoryLevel
            }).ToArray();

            return new JsonNetResult(new { categoryGroupsMembership = categoryGroupsMembership, AllCategoryGroups = allCategoryGroups });
        }

        [HttpPost]
        public JsonNetResult SetCategoryGroupsMembership(long organizationUnitId, string categoryGroupsMembership)
        {
            var deserializedData = 
                JsonConvert.DeserializeAnonymousType(
                    categoryGroupsMembership, 
                    new[] 
                        { 
                            new {
                                    Id = -1L, 
                                    CategoryId = -1L, 
                                    CategoryName = string.Empty, 
                                    CategoryGroupId = (long?)-1
                                }
                        }, 
                    new JsonSerializerSettings {Converters = {new Int64ToStringConverter()}});

            var categoryGroupMembershipDtos = deserializedData.Select(x => new CategoryGroupMembershipDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryGroupId = x.CategoryGroupId
            });

            _categoryService.SetCategoryGroupMembership(organizationUnitId, categoryGroupMembershipDtos);

            // javascript requires to return boolean property success=true
            return new JsonNetResult(new { success = true });
        }
    }
}
