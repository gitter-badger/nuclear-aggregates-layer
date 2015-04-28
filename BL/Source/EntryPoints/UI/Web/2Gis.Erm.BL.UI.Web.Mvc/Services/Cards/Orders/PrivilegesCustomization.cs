using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;

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
            var orderSecurityAspect = (IOrderSecurityAspect)viewModel;

            var currentUserCode = _userContext.Identity.Code;

            orderSecurityAspect.CurrenctUserCode = currentUserCode;
            orderSecurityAspect.CanEditOrderType = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditOrderType, currentUserCode);
        }
    }
}