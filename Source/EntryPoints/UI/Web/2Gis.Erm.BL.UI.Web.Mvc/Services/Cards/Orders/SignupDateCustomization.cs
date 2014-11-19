using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class SignupDateCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.SignupDate.Date < viewModel.CreatedOn.Date)
            {
                viewModel.SetWarning(BLResources.WarningOrderSignupDateLessThanCreationDate);
            }
        }
    }
}