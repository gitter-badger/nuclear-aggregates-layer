using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public sealed class CzechActivityViewModelCustomizationServiceHelper : ICzechAdapted
    {
        internal static void CustomizeViewModel<T>(IEntityViewModelBase viewModel, ModelStateDictionary modelState) where T : ActivityBase, new()
        {
            var entityViewModel = (CzechActivityBaseViewModelAbstract<T>)viewModel;
            string[] buttonsToDisable;

            switch (entityViewModel.Status)
            {
                case ActivityStatus.InProgress:
                    buttonsToDisable = new[] { "Revert" };
                    break;
                case ActivityStatus.Cancelled:
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