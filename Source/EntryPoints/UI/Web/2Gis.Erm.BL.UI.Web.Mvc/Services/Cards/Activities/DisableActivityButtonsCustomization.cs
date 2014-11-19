using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Activities
{
    public sealed class DisableActivityButtonsCustomization : IViewModelCustomization<ICustomizableActivityViewModel>
    {
        public void Customize(ICustomizableActivityViewModel viewModel, ModelStateDictionary modelState)
        {
            string[] buttonsToDisable;

            switch (viewModel.Status)
            {
                case ActivityStatus.InProgress:
                    buttonsToDisable = new[] { "Revert" };
                    break;
                case ActivityStatus.Canceled:
                case ActivityStatus.Completed:
                    buttonsToDisable = new[] { "Complete", "Cancel" };
                    viewModel.ViewConfig.ReadOnly = true;
                    break;
                default:
                    buttonsToDisable = new string[0];
                    break;
            }

            if (!viewModel.IsActive)
            {
                buttonsToDisable = new[] { "Complete", "Cancel", "Revert" };
            }

            var buttons = viewModel.ViewConfig.CardSettings.CardToolbar
                                         .Where(x => buttonsToDisable.Contains(x.Name)).ToArray();

            Array.ForEach(buttons, item => item.Disabled = true);
        }
    }
}