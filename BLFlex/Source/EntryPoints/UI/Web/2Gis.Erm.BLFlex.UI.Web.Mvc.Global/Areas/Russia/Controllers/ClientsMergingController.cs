using System.Linq;
using System.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Russia.Controllers
{
    public class ClientsMergingController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly IBusinessModelEntityObtainer<Client> _clientObtainer;

        public ClientsMergingController(IMsCrmSettings msCrmSettings, IAPIOperationsServiceSettings operationsServiceSettings, IAPISpecialOperationsServiceSettings specialOperationsServiceSettings, IAPIIdentityServiceSettings identityServiceSettings, IUserContext userContext, ICommonLog logger, IGetBaseCurrencyService getBaseCurrencyService, IOperationServicesManager operationServicesManager, ISecurityServiceUserIdentifier userIdentifierService, ISecurityServiceFunctionalAccess functionalAccessService, IPublicService publicService, IFinder finder, IBusinessModelEntityObtainer<Client> clientObtainer) : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _finder = finder;
            _clientObtainer = clientObtainer;
        }

        [HttpGetAttribute]
        public ActionResult Merge(long masterId, long? subordinateId, bool? disableMasterClient)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MergeClients, UserContext.Identity.Code))
            {
                throw new SecurityException(BLResources.AccessDeniedMergeClients);
            }

            // TODO {all, 01.02.2013}: Убрать получение DTO-объекта для ViewModel-и в агрегирующий репозиторий
            var model = new MergeClientsViewModel
                {
                    Client1 = _finder.Find<Client>(c => c.Id == masterId && !c.IsDeleted && c.IsActive)
                                     .Select(c => new LookupField { Key = c.Id, Value = c.Name })
                                     .Single(),
                    Client2 = subordinateId.HasValue
                                  ? _finder.Find<Client>(c => c.Id == subordinateId && !c.IsDeleted && c.IsActive)
                                           .Select(c => new LookupField { Key = c.Id, Value = c.Name })
                                           .Single()
                                  : null,
                    DisableMasterClient = disableMasterClient == true
                };
            model.SetWarning(BLResources.MergeRecordsNotification);
            return View(model);
        }

        [HttpPostAttribute]
        [GetEntityStateTokenAttribute]
        public virtual ActionResult Merge(ClientViewModel model)
        {
            var result = new MergeClientsViewModel();

            var domainEntityDto = (ClientDomainEntityDto)model.TransformToDomainEntityDto();
            domainEntityDto.OwnerRef = model.Owner.ToReference();

            var entity = _clientObtainer.ObtainBusinessModelEntity(domainEntityDto);
            entity.PromisingValue = model.PromisingScore;

            try
            {
                _publicService.Handle(new MergeClientsRequest
                    {
                        AppendedClientId = model.AppendedClient,
                        Client = entity,
                        AssignAllObjects = model.AssignAllObjects
                    });
                result.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                result.Client1 = _finder.Find<Client>(c => c.Id == entity.Id && !c.IsDeleted && c.IsActive)
                                        .Select(c => new LookupField { Key = c.Id, Value = c.Name })
                                        .Single();
                result.Client2 = _finder.Find<Client>(c => c.Id == model.AppendedClient && !c.IsDeleted && c.IsActive)
                                        .Select(c => new LookupField { Key = c.Id, Value = c.Name })
                                        .Single();
                result.SetCriticalError(ex.Message);
            }

            return View(result);
        }

        public ActionResult MergeClientsGetData(long masterId, long subordinateId)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MergeClients, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDeniedMergeClients);
            }

            var service = _operationServicesManager.GetDomainEntityDtoService(EntityName.Client);
            var masterClientDto = (ClientDomainEntityDto)service.GetDomainEntityDto(masterId, false, null, EntityName.None, string.Empty);
            var subordinateClientDto = (ClientDomainEntityDto)service.GetDomainEntityDto(subordinateId, false, null, EntityName.None, string.Empty);

            var masterClientModel = new ClientViewModel
                {
                    Owner = new LookupField
                        {
                            Key = masterClientDto.OwnerRef.Id,
                            Value = _userIdentifierService.GetUserInfo(masterClientDto.OwnerRef.Id).DisplayName
                        }
                };
            masterClientModel.LoadDomainEntityDto(masterClientDto);

            var subordinateIdClientModel = new ClientViewModel
                {
                    Owner = new LookupField
                        {
                            Key = subordinateClientDto.OwnerRef.Id,
                            Value = _userIdentifierService.GetUserInfo(subordinateClientDto.OwnerRef.Id).DisplayName
                        }
                };
            subordinateIdClientModel.LoadDomainEntityDto(subordinateClientDto);

            var model = new MergeClientsDataViewModel
                {
                    Client1 = masterClientModel,
                    Client2 = subordinateIdClientModel,
                };

            model.Client1.SetEntityStateToken();
            model.Client2.SetEntityStateToken();

            return View(model);
        }
    }
}