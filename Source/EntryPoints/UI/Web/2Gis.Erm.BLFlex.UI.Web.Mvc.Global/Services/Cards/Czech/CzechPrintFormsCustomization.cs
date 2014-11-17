using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Czech
{
    public sealed class CzechPrintFormsCustomization : IViewModelCustomization, ICzechAdapted
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            var isActionDisabledBasedOnWorkflowStepId = !entityViewModel.IsTerminated ||
                                                        !(entityViewModel.WorkflowStepId == (int)OrderState.OnTermination ||
                                                          entityViewModel.WorkflowStepId == (int)OrderState.Archive);

            if (isActionDisabledBasedOnWorkflowStepId)
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintAdditionalAgreementAction");
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeWithoutReasonAction");
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintBargainAdditionalAgreementAction");
            }
        }
    }
}