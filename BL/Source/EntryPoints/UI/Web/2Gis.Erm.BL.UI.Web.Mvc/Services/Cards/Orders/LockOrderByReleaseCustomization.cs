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
            var orderDirectionAspect = (IOrderDirectionAspect)viewModel;
            var orderWorkflowAspect = (IOrderWorkflowAspect)viewModel;
            var orderDatesAspect = (IOrderDatesAspect)viewModel;

            if (orderDirectionAspect.DestinationOrganizationUnitKey == null)
            {
                return;
            }

            if (orderWorkflowAspect.WorkflowStepId == OrderState.Approved || orderWorkflowAspect.WorkflowStepId == OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseReadModel.HasFinalReleaseInProgress(orderDirectionAspect.DestinationOrganizationUnitKey.Value,
                                                                                      new TimePeriod(orderDatesAspect.BeginDistributionDate, orderDatesAspect.EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    viewModel.LockOrderToolbar();
                    viewModel.SetWarning(BLResources.CannotEditOrderBecauseReleaseIsRunning);
                }
            }
        }
    }
}