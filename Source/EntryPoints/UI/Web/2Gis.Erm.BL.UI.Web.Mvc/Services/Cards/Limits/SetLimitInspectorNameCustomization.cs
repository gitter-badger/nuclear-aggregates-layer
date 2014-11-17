using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class SetLimitInspectorNameCustomization : IViewModelCustomization
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public SetLimitInspectorNameCustomization(ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LimitViewModel)viewModel;

            if (entityViewModel.Inspector != null)
            {
                entityViewModel.Inspector.Value = _userIdentifierService.GetUserInfo(entityViewModel.Inspector.Key).DisplayName;
            }
        }
    }
}
