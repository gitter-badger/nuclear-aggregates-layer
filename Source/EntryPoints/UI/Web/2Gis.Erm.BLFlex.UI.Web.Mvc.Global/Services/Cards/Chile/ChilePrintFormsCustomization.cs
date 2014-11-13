using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Chile
{
    public class ChilePrintFormsCustomization : IViewModelCustomization, IChileAdapted
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            var isActionDisabledBasedOnWorkflowStepId = !entityViewModel.IsTerminated ||
                                                        !(entityViewModel.WorkflowStepId == (int)OrderState.OnTermination ||
                                                          entityViewModel.WorkflowStepId == (int)OrderState.Archive);

            // TODO {y.baranihin, 13.11.2014}: сделать конфигурирование кнопок на отключение
            if (isActionDisabledBasedOnWorkflowStepId)
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                entityViewModel.ViewConfig.DisableCardToolbarItem("PrintBargainAdditionalAgreementAction");
            }
        }
    }
}