using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class OrderFileController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;

        public OrderFileController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IOperationServicesManager operationServicesManager,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
        }

        [HttpGet]
        public JsonNetResult GetSteObject(long id)
        {
            // ����� ���������� ���������� ��������� ��������, 
            // ��� ���������, ��� ������ ����������� ������� ����� �������� � ��������� ����� ��������.
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityName.OrderFile);
            var domainEntityDto = service.GetDomainEntityDto(id, false, null, EntityName.None, null);

            var model = new OrderFileViewModel();
            model.LoadDomainEntityDto(domainEntityDto);
            model.SetEntityStateToken();

            return new JsonNetResult(new { model.EntityStateToken });
        }
    }
}
