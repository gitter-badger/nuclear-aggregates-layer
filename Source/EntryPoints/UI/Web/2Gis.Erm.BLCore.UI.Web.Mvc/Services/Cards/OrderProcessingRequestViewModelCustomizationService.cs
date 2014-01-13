using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.Model.Entities;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Model.Entities.Erm;
using DoubleGis.Erm.Security.Interfaces;
using DoubleGis.Erm.Security.Interfaces.EntityAccess;
using DoubleGis.Erm.Security.Interfaces.UserContext;
using DoubleGis.Erm.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class OrderProcessingRequestViewModelCustomizationService : IGenericViewModelCustomizationService<OrderProcessingRequest>
    {
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;

        public OrderProcessingRequestViewModelCustomizationService(ISecurityServiceEntityAccess securityServiceEntityAccess, IUserContext userContext)
        {
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (OrderProcessingRequestViewModel)viewModel;

            var userId = _userContext.Identity.Code;
            entityViewModel.CanCreateOrder = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create, EntityName.Order, userId, null, userId, null);
            entityViewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityName.OrderPosition,
                                                                                           userId,
                                                                                           null,
                                                                                           userId,
                                                                                           null);

            entityViewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityName.OrderPositionAdvertisement,
                                                                                           userId,
                                                                                           null,
                                                                                           userId,
                                                                                           null);

            if (!entityViewModel.CanCreateOrder)
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("CreateOrder");
            }

            var deadEndStates = new[]
                {
                    OrderProcessingRequestState.Completed,
                    OrderProcessingRequestState.Cancelled
                };

            if (deadEndStates.Contains(entityViewModel.State))
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("CancelOrderProcessingRequest");
                entityViewModel.ViewConfig.DisableCardToolbarItem("CreateOrder");
            }
        }
    }
}