using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
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
            entityViewModel.CanCreateOrder = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create, EntityType.Instance.Order(), userId, null, userId, null);
            entityViewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityType.Instance.OrderPosition(),
                                                                                           userId,
                                                                                           null,
                                                                                           userId,
                                                                                           null);

            entityViewModel.CanCreateOrder &= _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Create,
                                                                                           EntityType.Instance.OrderPositionAdvertisement(),
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