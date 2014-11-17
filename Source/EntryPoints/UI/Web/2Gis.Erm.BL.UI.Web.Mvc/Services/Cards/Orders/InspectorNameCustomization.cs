using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class InspectorNameCustomization : IViewModelCustomization
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public InspectorNameCustomization(ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            entityViewModel.Inspector.Value = _userIdentifierService.GetUserInfo(entityViewModel.Inspector.Key).DisplayName;
        }
    }
}