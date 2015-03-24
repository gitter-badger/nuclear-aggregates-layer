using System.Web.Helpers;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class DisableOrderTypesOptionsCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public DisableOrderTypesOptionsCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            var disabledOrderTypesAspect = (IDisabledOrderTypesAspect)viewModel;

            disabledOrderTypesAspect.DisabledOrderTypes = Json.Encode(GetOrderTypesToDisable());
        }

        private string[] GetOrderTypesToDisable()
        {
            if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, _userContext.Identity.Code))
            {
                return new[]
                           {
                               OrderType.AdsBarter.ToString(),
                               OrderType.ProductAdsService.ToString(),
                               OrderType.ProductBarter.ToString(),
                               OrderType.SelfAds.ToString(),
                               OrderType.ServiceBarter.ToString(),
                               OrderType.SocialAds.ToString(),
                           };
            }
            else
            {
                return new[]
                           {
                               OrderType.AdvertisementAgency.ToString(),
                           };
            }
        }
    }
}