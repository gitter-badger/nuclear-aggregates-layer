using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;

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

            long firmOwnerCode;
            if (!_firmReadModel.TryGetFirmOwnerCodeUnsecure(firmId, out firmOwnerCode))
            {
                throw new EntityNotFoundException(typeof(Firm));
            }

            viewModel.ViewConfig.ReadOnly |= !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                               EntityName.Firm,
                                                                                               _userContext.Identity.Code,
                                                                                               firmId,
                                                                                               firmOwnerCode,
                                                                                               null);
        }
    }
}