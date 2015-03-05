using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class LockOrderByReleaseCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        private readonly IReleaseReadModel _releaseReadModel;

        public LockOrderByReleaseCustomization(IReleaseReadModel releaseReadModel)
        {
            _releaseReadModel = releaseReadModel;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {

            if (((IOrderDirectionAspect)viewModel).DestinationOrganizationUnitKey == null)
            {
                return;
            }

            if (((IOrderWorkflowAspect)viewModel).WorkflowStepId == OrderState.Approved || ((IOrderWorkflowAspect)viewModel).WorkflowStepId == OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseReadModel.HasFinalReleaseInProgress(((IOrderDirectionAspect)viewModel).DestinationOrganizationUnitKey.Value,
                                                                                      new TimePeriod(((IOrderDatesAspect)viewModel).BeginDistributionDate, ((IOrderDatesAspect)viewModel).EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    viewModel.LockOrderToolbar();
                    viewModel.SetWarning(BLResources.CannotEditOrderBecauseReleaseIsRunning);
                }
            }
        }
    }
}