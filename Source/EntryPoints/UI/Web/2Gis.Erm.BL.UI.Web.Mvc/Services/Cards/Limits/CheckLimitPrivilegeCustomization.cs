using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class CheckLimitPrivilegeCustomization : IViewModelCustomization
    {
        private readonly IUserContext _userContext;

        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public CheckLimitPrivilegeCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LimitViewModel)viewModel;

            entityViewModel.HasEditPeriodPrivelege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitPeriodChanging,
                                                                                                            _userContext.Identity.Code);
        }
    }
}