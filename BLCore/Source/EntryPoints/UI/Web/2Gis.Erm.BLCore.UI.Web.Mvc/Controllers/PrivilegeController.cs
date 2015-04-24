using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.Roles;
using DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class PrivilegeController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IRoleRepository _roleRepository;

        public PrivilegeController(IMsCrmSettings msCrmSettings,
                                   IAPIOperationsServiceSettings operationsServiceSettings,
                                   IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                   IAPIIdentityServiceSettings identityServiceSettings,
                                   IUserContext userContext,
                                   ITracer tracer,
                                   IGetBaseCurrencyService getBaseCurrencyService,
                                   ISecurityServiceFunctionalAccess functionalAccessService,
                                   IRoleRepository roleRepository)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _roleRepository = roleRepository;
        }

        #region entity privileges

        public JsonNetResult GetEntityPrivilegesForRole(long roleId)
        {
            var permissions = _roleRepository.GetEntityPrivileges(roleId);

            // long PrivilegeId парсятся в js с потерей точности, заменяем их на строки.
            var jsonPermissions = permissions
                .Select(x => new
                        {
                        x.EntityName.Description,
                            x.EntityNameLocalized,
                        PrivilegeInfoList = x.PrivilegeInfoList
                                             .Select(y => new
                                                 {
                                                     y.NameLocalized,
                                                     y.Operation,
                                                     y.PrivilegeDepthMask,
                                                     PrivilegeId = y.PrivilegeId.ToString()
                                                 })
                                             .ToArray()
                        });

            return new JsonNetResult(new { Data = jsonPermissions });
        }

        [HttpGet]
        public JsonNetResult GetEntityPrivilegesDepths()
        {
            var allEnumValues = (EntityPrivilegeDepthState[])typeof(EntityPrivilegeDepthState).GetEnumValues();

            var result = allEnumValues.Select(x => new
            {
                NameLocalized = x.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                Mask = x,
            }).ToArray();

            return new JsonNetResult(new { Data = result, RowCount = result.Count() });
        }

        [HttpPost]
        public JsonNetResult SaveEntityPrivileges(long roleId, IEntityType entityName, string data)
        {
            var entityPrivileges = JsonConvert.DeserializeObject<PrivilegeDto[]>(data);

            _roleRepository.UpdateEntityPrivileges(roleId, entityPrivileges);

            return new JsonNetResult();
        }

        #endregion

        #region functional privileges

        public JsonNetResult GetFunctionalPrivilegesForRole(long roleId)
        {
            var permissions = _roleRepository.GetFunctionalPrivileges(roleId);
            return new JsonNetResult(new { Data = permissions });
        }

        public JsonNetResult GetFunctionalPrivilegesDepths()
        {
            var data = _roleRepository.FindAllFunctionalPriveleges()
                .Select(x => new
                {
                    x.PrivilegeId,
                    NameLocalized = EnumResources.ResourceManager.GetString(x.NameLocalized) ?? x.NameLocalized,
                    x.Mask,
                    x.Priority
                })
                .ToList();

            data.Add(new
            {
                PrivilegeId = -1L,
                NameLocalized = EntityPrivilegeDepthState.None.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                Mask = 0,
                Priority = (byte)0
            });

            return new JsonNetResult(new { Data = data, RowCount = data.Count });
        }

        [HttpPost]
        public JsonNetResult SaveFunctionalPrivileges(long roleId, string data)
        {
            var functionalPrivileges = JsonConvert.DeserializeObject<FunctionalPrivilegeInfo[]>(data);

            _roleRepository.UpdateFunctionalPrivileges(roleId, functionalPrivileges);

            return new JsonNetResult();
        }

        [HttpGet]
        public JsonNetResult HasFunctionalPrivilegeGranted(string privilegeName)
        {
            FunctionalPrivilegeName privilege;
            if (Enum.TryParse(privilegeName, out privilege))
            {
                var result = _functionalAccessService.HasFunctionalPrivilegeGranted(privilege, UserContext.Identity.Code);
                return new JsonNetResult(result);
            }

            throw new NotificationException("Указанная привилегия не найдена");
        }

        #endregion
    }
}
