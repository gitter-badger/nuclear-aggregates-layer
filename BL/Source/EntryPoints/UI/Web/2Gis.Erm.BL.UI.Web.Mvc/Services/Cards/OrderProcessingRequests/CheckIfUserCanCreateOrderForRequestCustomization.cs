using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderProcessingRequests
{
    public sealed class CheckIfUserCanCreateOrderForRequestCustomization : IViewModelCustomization<OrderProcessingRequestViewModel>
    {
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;
         
        public CheckIfUserCanCreateOrderForRequestCustomization(ISecurityServiceEntityAccess securityServiceEntityAccess, IUserContext userContext)
        {
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
        }

        public void Customize(OrderProcessingRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            var userId = _userContext.Identity.Code;
            viewModel.CanCreateOrder = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create, EntityName.Order, userId, null, userId, null);
            viewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityName.OrderPosition,
                                                                                           userId,
                                                                                           null,
                                                                                           userId,
                                                                                           null);

            viewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityName.OrderPositionAdvertisement,
                                                                                           userId,
                                                                                           null,
                                                                                           userId,
                                                                                           null);

            if (!viewModel.CanCreateOrder)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("CreateOrder");
            }
        }
    }
}
