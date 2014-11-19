using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements
{
    public sealed class AdvertisementAccessCustomization : IViewModelCustomization<AdvertisementViewModel>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IFirmReadModel _firmReadModel;

        public AdvertisementAccessCustomization(IUserContext userContext, ISecurityServiceEntityAccess securityServiceEntityAccess, IFirmReadModel firmReadModel)
        {
            _userContext = userContext;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _firmReadModel = firmReadModel;
        }

        public void Customize(AdvertisementViewModel viewModel, ModelStateDictionary modelState)
        {
            if (!viewModel.Firm.Key.HasValue)
            {
                return;
            }

            var firmId = viewModel.Firm.Key.Value;
            var firmOwnerCode = _firmReadModel.GetFirmOwnerCodeUnsecure(firmId);

            viewModel.ViewConfig.ReadOnly |= !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                           EntityName.Firm,
                                                                                           _userContext.Identity.Code,
                                                                                           firmId,
                                                                                           firmOwnerCode,
                                                                                           null);
        }
    }
}