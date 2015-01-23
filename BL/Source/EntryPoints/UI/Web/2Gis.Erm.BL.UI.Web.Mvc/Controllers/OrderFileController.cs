using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class OrderFileController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;

        public OrderFileController(IMsCrmSettings msCrmSettings,
                                   IUserContext userContext,
                                   ICommonLog logger,
                                   IOperationServicesManager operationServicesManager,
                                   IAPIOperationsServiceSettings operationsServiceSettings,
                                   IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                   IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
        }

        [HttpGet]
        public JsonNetResult GetSteObject(long id)
        {
            // ����� ���������� ���������� ��������� ��������, 
            // ��� ���������, ��� ������ ����������� ������� ����� �������� � ��������� ����� ��������.
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityType.Instance.OrderFile());
            var domainEntityDto = service.GetDomainEntityDto(id, false, null, EntityType.Instance.None(), null);

            var model = new OrderFileViewModel();
            model.LoadDomainEntityDto(domainEntityDto);
            model.SetEntityStateToken();

            return new JsonNetResult(new { model.EntityStateToken });
        }
    }
}
