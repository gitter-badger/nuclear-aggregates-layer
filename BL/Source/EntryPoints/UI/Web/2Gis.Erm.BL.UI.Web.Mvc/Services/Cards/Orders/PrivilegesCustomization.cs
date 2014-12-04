using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class PrivilegesCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public PrivilegesCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            var currentUserCode = _userContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => _functionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            viewModel.CurrenctUserCode = currentUserCode;

            viewModel.CanEditOrderType = functionalPrivilegeValidator(FunctionalPrivilegeName.EditOrderType);
            viewModel.HasOrderBranchOfficeOrganizationUnitSelection =
                functionalPrivilegeValidator(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection);
            viewModel.HasOrderDocumentsDebtChecking =
                _functionalAccessService.HasOrderChangeDocumentsDebtAccess(viewModel.SourceOrganizationUnit.Key ?? 0, currentUserCode);

            // Если есть права и нет сборки в настоящий момент 
            viewModel.HasOrderDocumentsDebtChecking &= !viewModel.IsWorkflowLocked;

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderChangeDealExtended, currentUserCode))
            {
                viewModel.ViewConfig.DisableCardToolbarItem("ChangeDeal", false);
            }
        }
    }
}