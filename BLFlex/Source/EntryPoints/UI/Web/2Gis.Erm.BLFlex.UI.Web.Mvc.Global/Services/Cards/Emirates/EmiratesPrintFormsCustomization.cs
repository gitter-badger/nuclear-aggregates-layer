using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Emirates
{
    public sealed class EmiratesPrintFormsCustomization : IViewModelCustomization<EntityViewModelBase<Order>>, IEmiratesAdapted
    {
        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            var isActionDisabledBasedOnWorkflowStepId = !((ITerminatableAspect)viewModel).IsTerminated ||
                                                        !(((IOrderWorkflowAspect)viewModel).WorkflowStepId == OrderState.OnTermination ||
                                                          ((IOrderWorkflowAspect)viewModel).WorkflowStepId == OrderState.Archive);

            if (isActionDisabledBasedOnWorkflowStepId)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                viewModel.ViewConfig.DisableCardToolbarItem("PrintAdditionalAgreementAction");
                viewModel.ViewConfig.DisableCardToolbarItem("PrintBargainAdditionalAgreementAction");
            }
        }
    }
}