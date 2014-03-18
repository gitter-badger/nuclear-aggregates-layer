﻿using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdditionalFirmServiceController : ControllerBase
    {
        private readonly IFirmAdditionalServiceOperations _serviceOperations;

        public AdditionalFirmServiceController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IFirmAdditionalServiceOperations serviceOperations,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
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