using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class SignupDateCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            if (((IOrderDatesAspect)viewModel).SignupDate.Date < viewModel.CreatedOn.Date)
            {
                viewModel.SetWarning(BLResources.WarningOrderSignupDateLessThanCreationDate);
            }
        }
    }
}