using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Chile
{
    public class ChilePrintFormsCustomization : IViewModelCustomization<ICustomizableOrderViewModel>, IChileAdapted
    {
        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            var isActionDisabledBasedOnWorkflowStepId = !viewModel.IsTerminated ||
                                                        !(viewModel.WorkflowStepId == (int)OrderState.OnTermination ||
                                                          viewModel.WorkflowStepId == (int)OrderState.Archive);

            if (isActionDisabledBasedOnWorkflowStepId)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                viewModel.ViewConfig.DisableCardToolbarItem("PrintBargainAdditionalAgreementAction");
            }
        }
    }
}