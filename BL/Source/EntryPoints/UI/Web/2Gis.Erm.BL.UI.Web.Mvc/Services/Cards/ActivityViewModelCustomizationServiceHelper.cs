using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public static class ActivityViewModelCustomizationServiceHelper
    {
        private static class ToolbarItem
        {
            public const string Complete = "Complete";
            public const string Cancel = "Cancel";
            public const string Revert = "Revert";
        }

        internal static void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IActivityViewModel)viewModel;
            var buttonsToDisable = new List<string>(3);

            if (viewModel.IsActive)
            {
                switch (entityViewModel.Status)
                {
                    case ActivityStatus.InProgress:
                        buttonsToDisable.Add(ToolbarItem.Revert);
                        break;
                    case ActivityStatus.Canceled:
                    case ActivityStatus.Completed:
                        buttonsToDisable.Add(ToolbarItem.Complete);
                        buttonsToDisable.Add(ToolbarItem.Cancel);
                        viewModel.ViewConfig.ReadOnly = true;
                        break;
                }
            }
            else
            {
                buttonsToDisable.AddRange(new[] { ToolbarItem.Complete, ToolbarItem.Cancel, ToolbarItem.Revert });
            }

            foreach (var button in viewModel.ViewConfig.CardSettings.CardToolbar.Where(x => buttonsToDisable.Contains(x.Name)))
            {
                button.Disabled = true;
            }
        }
    }
}