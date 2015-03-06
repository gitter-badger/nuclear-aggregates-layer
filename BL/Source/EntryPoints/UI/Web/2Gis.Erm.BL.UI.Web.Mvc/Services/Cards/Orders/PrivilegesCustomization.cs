using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class PrivilegesCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public PrivilegesCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            var orderDirectionAspect = (IOrderDirectionAspect)viewModel;
            var orderSecurityAspect = (IOrderSecurityAspect)viewModel;
            var orderWorkflowLockableAspect = (IOrderWorkflowLockableAspect)viewModel;

            var currentUserCode = _userContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => _functionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            orderSecurityAspect.CurrenctUserCode = currentUserCode;

            orderSecurityAspect.CanEditOrderType = functionalPrivilegeValidator(FunctionalPrivilegeName.EditOrderType);
            orderSecurityAspect.HasOrderDocumentsDebtChecking =
                _functionalAccessService.HasOrderChangeDocumentsDebtAccess(orderDirectionAspect.SourceOrganizationUnitKey ?? 0, currentUserCode);

            // ���� ���� ����� � ��� ������ � ��������� ������ 
            orderSecurityAspect.HasOrderDocumentsDebtChecking &= !orderWorkflowLockableAspect.IsWorkflowLocked;
        }
    }
}