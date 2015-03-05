using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class InspectorNameCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public InspectorNameCustomization(ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            ((IInspectorAspect)viewModel).InspectorValue = _userIdentifierService.GetUserInfo(((IInspectorAspect)viewModel).InspectorKey).DisplayName;
        }
    }
}