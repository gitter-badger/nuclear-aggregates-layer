using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class LockLimitByWorkflowCustomization : IViewModelCustomization<LimitViewModel>
    {
        public void Customize(LimitViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.Status != LimitStatus.Opened)
            {
                viewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}
