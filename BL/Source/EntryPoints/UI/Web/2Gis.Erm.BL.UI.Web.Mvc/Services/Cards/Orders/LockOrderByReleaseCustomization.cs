using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class LockOrderByReleaseCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        private readonly IReleaseReadModel _releaseReadModel;

        public LockOrderByReleaseCustomization(IReleaseReadModel releaseReadModel)
        {
            _releaseReadModel = releaseReadModel;
        }

        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {

            if (viewModel.DestinationOrganizationUnit == null || viewModel.DestinationOrganizationUnit.Key == null)
            {
                return;
            }

            if (viewModel.WorkflowStepId == (int)OrderState.Approved || viewModel.WorkflowStepId == (int)OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseReadModel.HasFinalReleaseInProgress(viewModel.DestinationOrganizationUnit.Key.Value,
                                                                                      new TimePeriod(viewModel.BeginDistributionDate, viewModel.EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    viewModel.LockToolbar();
                    viewModel.SetWarning(BLResources.CannotEditOrderBecauseReleaseIsRunning);
                }
            }
        }
    }
}