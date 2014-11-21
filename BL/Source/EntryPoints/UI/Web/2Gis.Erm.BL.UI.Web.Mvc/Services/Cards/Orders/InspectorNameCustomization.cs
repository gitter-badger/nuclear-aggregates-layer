using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Security;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class InspectorNameCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public InspectorNameCustomization(ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
        }

        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.Inspector.Value = _userIdentifierService.GetUserInfo(viewModel.Inspector.Key).DisplayName;
        }
    }
}