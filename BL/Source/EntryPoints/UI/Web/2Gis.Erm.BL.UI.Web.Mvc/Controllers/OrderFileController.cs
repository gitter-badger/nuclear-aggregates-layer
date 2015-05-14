using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class OrderFileController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;

        public OrderFileController(IMsCrmSettings msCrmSettings,
                                   IAPIOperationsServiceSettings operationsServiceSettings,
                                   IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                   IAPIIdentityServiceSettings identityServiceSettings,
                                   IUserContext userContext,
                                   ITracer tracer,
                                   IGetBaseCurrencyService getBaseCurrencyService,
                                   IOperationServicesManager operationServicesManager)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
        }

        [HttpGet]
        public JsonNetResult GetSteObject(long id)
        {
            // Метод возвращает обновлённое состояние сущности, 
            // это требуется, ибо сейчас асинхронная заливка файла приводит к изменению самой сущности.
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityType.Instance.OrderFile());
            var domainEntityDto = service.GetDomainEntityDto(id, false, null, EntityType.Instance.None(), null);

            var model = new OrderFileViewModel();
            model.LoadDomainEntityDto(domainEntityDto);
            model.SetEntityStateToken();

            return new JsonNetResult(new { model.EntityStateToken });
        }
    }
}
