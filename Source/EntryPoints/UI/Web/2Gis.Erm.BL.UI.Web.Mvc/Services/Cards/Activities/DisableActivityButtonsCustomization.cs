using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Activities
{
    public sealed class DisableActivityButtonsCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ICustomizableActivityViewModel)viewModel;
            string[] buttonsToDisable;

            switch (entityViewModel.Status)
            {
                case ActivityStatus.InProgress:
                    buttonsToDisable = new[] { "Revert" };
                    break;
                case ActivityStatus.Canceled:
                case ActivityStatus.Completed:
                    buttonsToDisable = new[] { "Complete", "Cancel" };
                    entityViewModel.ViewConfig.ReadOnly = true;
                    break;
                default:
                    buttonsToDisable = new string[0];
                    break;
            }

            if (!viewModel.IsActive)
            {
                buttonsToDisable = new[] { "Complete", "Cancel", "Revert" };
            }

            var buttons = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                         .Where(x => buttonsToDisable.Contains(x.Name)).ToArray();

            Array.ForEach(buttons, item => item.Disabled = true);
        }
    }
}