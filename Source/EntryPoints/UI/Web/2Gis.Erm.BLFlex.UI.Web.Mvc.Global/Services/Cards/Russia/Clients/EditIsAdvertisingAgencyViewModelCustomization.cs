﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Clients
{
    public sealed class EditIsAdvertisingAgencyViewModelCustomization : IViewModelCustomization, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public EditIsAdvertisingAgencyViewModelCustomization(ISecurityServiceFunctionalAccess functionalAccessService, IUserContext userContext)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var clientViewModel = viewModel as ClientViewModel;
            if (clientViewModel == null)
            {
                return;
            }

            clientViewModel.CanEditIsAdvertisingAgency = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement,
                                                                                                                _userContext.Identity.Code);
        }
    }
}