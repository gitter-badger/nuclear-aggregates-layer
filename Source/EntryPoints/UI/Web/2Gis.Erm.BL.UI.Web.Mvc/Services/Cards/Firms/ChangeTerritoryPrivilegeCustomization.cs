using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms
{
    public sealed class ChangeTerritoryPrivilegeCustomization : IViewModelCustomization
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ChangeTerritoryPrivilegeCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ChangeFirmTerritory,
                                                                                      _userContext.Identity.Code))
            {
                viewModel.ViewConfig.CardSettings.CardToolbar =
                    viewModel.ViewConfig.CardSettings.CardToolbar
                                   .Where(x => !string.Equals(x.Name, "ChangeTerritory", StringComparison.Ordinal))
                                   .ToArray();
            }
        }
    }
}