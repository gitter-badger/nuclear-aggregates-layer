using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdditionalFirmServiceController : ControllerBase
    {
        private readonly IFirmAdditionalServiceOperations _serviceOperations;

        public AdditionalFirmServiceController(IMsCrmSettings msCrmSettings,
                                               IAPIOperationsServiceSettings operationsServiceSettings,
                                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                               IAPIIdentityServiceSettings identityServiceSettings,
                                               IUserContext userContext,
                                               ITracer tracer,
                                               IGetBaseCurrencyService getBaseCurrencyService,
                                               IFirmAdditionalServiceOperations serviceOperations)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _serviceOperations = serviceOperations;
        }

        #region firm services

        [HttpGet]
        public JsonNetResult GetAdditionalFirmServices(long firmId)
        {
            var additionalServicesDtos = _serviceOperations.GetFirmAdditionalServices(firmId);
            var data = additionalServicesDtos.Select(x => new
            {
                Name = x.ServiceCode,
                NameLocalized = x.Description,
                Display = x.DisplayService,
            }).ToArray();

            return new JsonNetResult(new { data });
        }

        [HttpPost]
        public JsonNetResult SetAdditionalFirmServices(long firmId, string data)
        {
            var deserializedData = JsonConvert.DeserializeAnonymousType(data, new[] { new { Name = (string)null, Display = AdditionalServiceDisplay.Default } });
            var additionalServicesDtos = deserializedData.Select(x => new AdditionalServicesDto { ServiceCode = x.Name, DisplayService = x.Display });

            _serviceOperations.SetFirmServices(firmId, additionalServicesDtos);

            // javascript requires to return boolean property success=true
            return new JsonNetResult(new { success = true });
        }

        #endregion

        #region firm address services

        [HttpGet]
        public JsonNetResult GetAdditionalFirmAddressServices(long firmAddressId)
        {
            var additionalServicesDtos = _serviceOperations.GetFirmAddressAdditionalServices(firmAddressId);
            var data = additionalServicesDtos.Select(x => new
            {
                Name = x.ServiceCode,
                NameLocalized = x.Description,
                Display = x.DisplayService,
            }).ToArray();
            
            return new JsonNetResult(new { data });
        }

        [HttpPost]
        public JsonNetResult SetAdditionalFirmAddressServices(long firmAddressId, string data)
        {
            var deserializedData = JsonConvert.DeserializeAnonymousType(data, new[] { new { Name = (string)null, Display = AdditionalServiceDisplay.Default } });
            var additionalServicesDtos = deserializedData.Select(x => new AdditionalServicesDto { ServiceCode = x.Name, DisplayService = x.Display });

            _serviceOperations.SetFirmAddressServices(firmAddressId, additionalServicesDtos);

            // javascript requires to return boolean property success=true
            return new JsonNetResult(new { success = true });
        }

        #endregion
    }
}