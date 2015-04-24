using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Orders
{
    public sealed class ManageDocumentsDebtCustomization : IViewModelCustomization<EntityViewModelBase<Order>>, IRussiaAdapted
    {
        private const string ButtonName = "SetDocumentsDebt";
        private readonly IUserContext _userContext;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ManageDocumentsDebtCustomization(IUserContext userContext, IReleaseReadModel releaseReadModel, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _releaseReadModel = releaseReadModel;
            _functionalAccessService = functionalAccessService;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                viewModel.ViewConfig.CardSettings.CardToolbar = HideButton(viewModel.ViewConfig.CardSettings.CardToolbar);
                return;
            }

            var orderDirectionAspect = (IOrderDirectionAspect)viewModel;
            var orderDatesAspect = (IOrderDatesAspect)viewModel;

            if (!_functionalAccessService.HasOrderChangeDocumentsDebtAccess(orderDirectionAspect.SourceOrganizationUnitKey.Value, _userContext.Identity.Code))
            {
                viewModel.ViewConfig.CardSettings.CardToolbar = HideButton(viewModel.ViewConfig.CardSettings.CardToolbar);
                return;
            }

            if (_releaseReadModel.HasFinalReleaseInProgress(orderDirectionAspect.DestinationOrganizationUnitKey.Value,
                                                            new TimePeriod(orderDatesAspect.BeginDistributionDate, orderDatesAspect.EndDistributionDateFact.GetEndPeriodOfThisMonth())))
            {
                DisableButton(viewModel.ViewConfig.CardSettings.CardToolbar);
            }
        }

        private static ToolbarElementStructure[] HideButton(IEnumerable<ToolbarElementStructure> cardToolbar)
        {
            return cardToolbar.Where(x => x.Name != ButtonName).ToArray();
        }

        private static void DisableButton(IEnumerable<ToolbarElementStructure> cardToolbar)
        {
            var button = cardToolbar.FirstOrDefault(x => x.Name == ButtonName);
            if (button != null)
            {
                button.Disabled = true;
            }
        }
    }
}
