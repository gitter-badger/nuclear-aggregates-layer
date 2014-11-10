using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public class PrivilegesCustomization : IViewModelCustomization
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public PrivilegesCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            var currentUserCode = _userContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => _functionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            entityViewModel.CurrenctUserCode = currentUserCode;

            entityViewModel.CanEditOrderType = functionalPrivilegeValidator(FunctionalPrivilegeName.EditOrderType);
            entityViewModel.HasOrderBranchOfficeOrganizationUnitSelection =
                functionalPrivilegeValidator(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection);
            entityViewModel.EditRegionalNumber = functionalPrivilegeValidator(FunctionalPrivilegeName.MakeRegionalAdsDocs);
            entityViewModel.HasOrderDocumentsDebtChecking =
                _functionalAccessService.HasOrderChangeDocumentsDebtAccess(entityViewModel.SourceOrganizationUnit.Key ?? 0, currentUserCode);

            // Если есть права и нет сборки в настоящий момент 
            entityViewModel.HasOrderDocumentsDebtChecking &= !entityViewModel.IsWorkflowLocked;

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderChangeDealExtended, currentUserCode))
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("ChangeDeal", false);
            }
        }
    }
}