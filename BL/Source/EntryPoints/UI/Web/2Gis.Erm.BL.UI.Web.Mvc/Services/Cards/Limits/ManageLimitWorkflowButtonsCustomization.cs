using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class ManageLimitWorkflowButtonsCustomization : IViewModelCustomization<LimitViewModel>
    {
        public void Customize(LimitViewModel viewModel, ModelStateDictionary modelState)
        {
            var toolbar = viewModel.ViewConfig.CardSettings.CardToolbar.ToArray();

            if (viewModel.IsNew)
            {
                DisableButtons(toolbar, new[] { "OpenLimit", "RejectLimit", "ApproveLimit", "RecalculateLimit" });
                return;
            }

            switch (viewModel.Status)
            {
                case LimitStatus.Opened:
                    DisableButtons(toolbar, new[] { "OpenLimit" });
                    break;
                case LimitStatus.Approved:
                case LimitStatus.Rejected:
                    DisableButtons(toolbar, new[] { "RejectLimit", "ApproveLimit" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("model");
            }
        }

        private static void DisableButtons(IEnumerable<ToolbarJson> toolbar, IEnumerable<string> statusButtons)
        {
            var buttonsToDisable = toolbar.Where(item => statusButtons.Contains(item.Name, StringComparer.OrdinalIgnoreCase));
            foreach (var item in buttonsToDisable)
            {
                item.Disabled = true;
            }
        }
    }
}