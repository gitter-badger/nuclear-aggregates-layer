using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Security;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class SetLimitInspectorNameCustomization : IViewModelCustomization<LimitViewModel>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public SetLimitInspectorNameCustomization(ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
        }

        public void Customize(LimitViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.Inspector != null)
            {
                viewModel.Inspector.Value = _userIdentifierService.GetUserInfo(viewModel.Inspector.Key).DisplayName;
            }
        }
    }
}
