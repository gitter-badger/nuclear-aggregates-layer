using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class LockLimitByWorkflowCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LimitViewModel)viewModel;

            if (entityViewModel.Status != LimitStatus.Opened)
            {
                entityViewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}
