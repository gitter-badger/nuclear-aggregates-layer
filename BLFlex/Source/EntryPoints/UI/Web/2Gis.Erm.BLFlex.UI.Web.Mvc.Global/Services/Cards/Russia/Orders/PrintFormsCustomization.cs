﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Orders
{
    public sealed class PrintFormsCustomization : IViewModelCustomization<EntityViewModelBase<Order>>, IRussiaAdapted
    {
        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            var orderWorkflowAspect = (IOrderWorkflowAspect)viewModel;
            var terminatableAspect = (ITerminatableAspect)viewModel;

            var isActionDisabledBasedOnWorkflowStepId = !terminatableAspect.IsTerminated ||
                                                        !(orderWorkflowAspect.WorkflowStepId == OrderState.OnTermination ||
                                                          orderWorkflowAspect.WorkflowStepId == OrderState.Archive);
            
            if (isActionDisabledBasedOnWorkflowStepId)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                viewModel.ViewConfig.DisableCardToolbarItem("PrintAdditionalAgreementAction");
            }
        }
    }
}
