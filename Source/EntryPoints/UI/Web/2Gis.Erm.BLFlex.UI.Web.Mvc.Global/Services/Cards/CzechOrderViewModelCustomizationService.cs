using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Newtonsoft.Json;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class CzechOrderViewModelCustomizationService : IGenericViewModelCustomizationService<Order>, ICzechAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IAPIOrderValidationServiceSettings _orderValidationServiceSettings;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecureFinder _secureFinder;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IPublicService _publicService;

        public CzechOrderViewModelCustomizationService(IUserContext userContext,
                                                       IAPIOrderValidationServiceSettings orderValidationServiceSettings,
                                                       ISecurityServiceFunctionalAccess functionalAccessService,
                                                       ISecurityServiceUserIdentifier userIdentifierService,
                                                       ISecureFinder secureFinder,
                                                       IReleaseReadModel releaseReadModel,
                                                       IPublicService publicService)
        {
            _userContext = userContext;
            _orderValidationServiceSettings = orderValidationServiceSettings;
            _functionalAccessService = functionalAccessService;
            _userIdentifierService = userIdentifierService;
            _secureFinder = secureFinder;
            _releaseReadModel = releaseReadModel;
            _publicService = publicService;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var currentUserCode = _userContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => _functionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            var entityViewModel = (CzechOrderViewModel)viewModel;

            modelState.SetModelValue("WorkflowStepId", new ValueProviderResult(entityViewModel.PreviousWorkflowStepId, entityViewModel.PreviousWorkflowStepId.ToString(CultureInfo.InvariantCulture), null));

            entityViewModel.OrderValidationServiceUrl = _orderValidationServiceSettings.RestUrl;

            entityViewModel.CurrenctUserCode = currentUserCode;
            entityViewModel.Inspector.Value = _userIdentifierService.GetUserInfo(entityViewModel.Inspector.Key).DisplayName;
            entityViewModel.AvailableSteps = GetAvailableSteps(entityViewModel.Id, entityViewModel.IsNew, (OrderState)entityViewModel.WorkflowStepId, entityViewModel.SourceOrganizationUnit.Key);

            // Проверить функциональные разрешения
            entityViewModel.HasOrderCreationExtended = functionalPrivilegeValidator(FunctionalPrivilegeName.OrderCreationExtended);
            entityViewModel.CanEditOrderType = functionalPrivilegeValidator(FunctionalPrivilegeName.EditOrderType);
            entityViewModel.HasOrderBranchOfficeOrganizationUnitSelection = functionalPrivilegeValidator(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection);
            entityViewModel.EditRegionalNumber = functionalPrivilegeValidator(FunctionalPrivilegeName.MakeRegionalAdsDocs);

            entityViewModel.HasOrderDocumentsDebtChecking = _functionalAccessService.HasOrderChangeDocumentsDebtAccess(entityViewModel.SourceOrganizationUnit.Key ?? 0, currentUserCode);

            // Если есть права и нет сборки в настоящий момент 
            entityViewModel.HasOrderDocumentsDebtChecking &= !entityViewModel.IsWorkflowLocked;

            // Карточка только на чтение если не "на регистрациии"
            entityViewModel.ViewConfig.ReadOnly = entityViewModel.WorkflowStepId != (int)OrderState.OnRegistration;

            if (!entityViewModel.IsActive)
            {
                entityViewModel.MessageType = MessageType.Warning;
                entityViewModel.Message = BLResources.WarningOrderIsRejected;
            }
            else if (entityViewModel.SignupDate.Date < entityViewModel.CreatedOn.Date)
            {
                entityViewModel.MessageType = MessageType.Warning;
                entityViewModel.Message = BLResources.WarningOrderSignupDateLessThanCreationDate;
            }

            if (entityViewModel.IsNew)
            {
                return;
            }

            if (!entityViewModel.IsActive)
            {
                LockToolbar(entityViewModel);
                return;
            }

            if (entityViewModel.WorkflowStepId == (int)OrderState.Approved || entityViewModel.WorkflowStepId == (int)OrderState.OnTermination)
            {
                if (!entityViewModel.DestinationOrganizationUnit.Key.HasValue)
                {
                    throw new NotificationException("Destination organization unit should be specified");
                }

                var isReleaseInProgress = _releaseReadModel.HasFinalReleaseInProgress(entityViewModel.DestinationOrganizationUnit.Key.Value, new TimePeriod(entityViewModel.BeginDistributionDate, entityViewModel.EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    LockToolbar(entityViewModel);
                    entityViewModel.SetWarning(BLResources.CannotEditOrderBecauseReleaseIsRunning);
                }
            }

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderChangeDealExtended, currentUserCode))
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("ChangeDeal", false);
            }

            // Кнопки изменения договора закрыты при наличии сборки
            var hasLocks = _secureFinder.Find<Order>(order => order.Id == entityViewModel.Id).SelectMany(order => order.Locks).Any(@lock => !@lock.IsDeleted);
            if (hasLocks)
            {
                var bargainButtons = entityViewModel.ViewConfig.CardSettings.CardToolbar
                    .Where(x => string.Equals(x.Name, "CreateBargain", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, "RemoveBargain", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                Array.ForEach(bargainButtons, item => item.Disabled = true);
            }

            {   // restrict printing of termination notice and additional agreement
                var isActionDisabledBasedOnWorkflowStepId = !entityViewModel.IsTerminated || !(entityViewModel.WorkflowStepId == (int)OrderState.OnTermination || entityViewModel.WorkflowStepId == (int)OrderState.Archive);
                if (isActionDisabledBasedOnWorkflowStepId)
                {
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeWithoutReasonAction");
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationBargainNoticeAction");
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationBargainNoticeWithoutReasonAction");
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintBargainAdditionalAgreementAction");
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintAdditionalAgreementAction", false);
                }

                var isReqionalOrder = entityViewModel.SourceOrganizationUnit.Key !=
                                      entityViewModel.DestinationOrganizationUnit.Key;

                if (isActionDisabledBasedOnWorkflowStepId || !isReqionalOrder)
                {
                    entityViewModel.ViewConfig.DisableCardToolbarItem("PrintRegTerminationNoticeAction", false);
                }
            }

            var disableCheckOrder = !(entityViewModel.WorkflowStepId == (int)OrderState.OnApproval || entityViewModel.WorkflowStepId == (int)OrderState.Approved || entityViewModel.WorkflowStepId == (int)OrderState.OnRegistration);
            if (disableCheckOrder)
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("CheckOrder");
            }
            else
            {
                entityViewModel.ViewConfig.EnableCardToolbarItem("CheckOrder");
            }
        }

        private static void LockToolbar(CzechOrderViewModel orderViewModel)
        {
            Array.ForEach(orderViewModel.ViewConfig.CardSettings.CardToolbar.ToArray(), item => item.Disabled = true);
            orderViewModel.IsWorkflowLocked = true;
        }

        private string GetAvailableSteps(long orderId, bool isNew, OrderState currentState, long? sourceOrganizationUnitId)
        {
            var resultList = new List<OrderState> { currentState };

            if (!isNew)
            {
                var response = (AvailableTransitionsResponse)_publicService.Handle(new AvailableTransitionsRequest
                    {
                        OrderId = orderId,
                        CurrentState = currentState,
                        SourceOrganizationUnitId = sourceOrganizationUnitId
                    });
                resultList.AddRange(response.AvailableTransitions);
            }

            return JsonConvert.SerializeObject(resultList.ConvertAll(state => new
                {
                    Value = state.ToString("D"),
                    Text = state.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                }));
        }
    }
}