using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    // 2+: BLFlex\Source\EntryPoints\UI\Web\2Gis.Erm.BLFlex.Web.Mvc.Global\Services\Cards\ClientViewModelCustomizationService.cs
    public class ClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ClientViewModelCustomizationService(ISecurityServiceFunctionalAccess functionalAccessService, IUserContext userContext)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ClientViewModel)viewModel;

            entityViewModel.CanEditIsAdvertisingAgency =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement,
                                                                       _userContext.Identity.Code);

            if (entityViewModel.IsNew)
            {
                // При создании нового клиента должна проставляться дата взятия из резерва, но не дата возвращения в резерв.
                entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
            }
        }
    }
}