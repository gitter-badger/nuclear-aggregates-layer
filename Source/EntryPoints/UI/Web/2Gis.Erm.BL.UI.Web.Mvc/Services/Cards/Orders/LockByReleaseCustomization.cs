using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public class LockByReleaseCustomization : IViewModelCustomization
    {
        private readonly IReleaseReadModel _releaseReadModel;

        public LockByReleaseCustomization(IReleaseReadModel releaseReadModel)
        {
            _releaseReadModel = releaseReadModel;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            if (entityViewModel.DestinationOrganizationUnit == null || entityViewModel.DestinationOrganizationUnit.Key == null)
            {
                return;
            }

            if (entityViewModel.WorkflowStepId == (int)OrderState.Approved || entityViewModel.WorkflowStepId == (int)OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseReadModel.HasFinalReleaseInProgress(entityViewModel.DestinationOrganizationUnit.Key.Value,
                                                                                      new TimePeriod(entityViewModel.BeginDistributionDate, entityViewModel.EndDistributionDateFact));
                if (isReleaseInProgress)
                {
                    entityViewModel.LockToolbar();
                    entityViewModel.SetWarning(BLResources.CannotEditOrderBecauseReleaseIsRunning);
                }
            }
        }
    }
}