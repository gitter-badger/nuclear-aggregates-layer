using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class CheckIfCanSwitchToAccountCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public CheckIfCanSwitchToAccountCustomization(IAccountReadModel accountReadModel, ISecurityServiceEntityAccess entityAccessService, IUserContext userContext)
        {
            _accountReadModel = accountReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            if (!CanSwitchToAccount(viewModel.Id))
            {
                // TODO {all, 26.01.2015}: Сделать словарик с именами элементов
                viewModel.ViewConfig.DisableCardToolbarItem("SwitchToAccount");
            }
        }

        private bool CanSwitchToAccount(long orderId)
        {
            var accountInfo = _accountReadModel.GetAccountIdAndOwnerCodeByOrder(orderId);
            if (accountInfo == null || !accountInfo.AccountId.HasValue)
            {
                return false;
            }

            var identitySecurityControlAspect = _userContext.Identity as IUserIdentitySecurityControl;
            if (identitySecurityControlAspect != null && identitySecurityControlAspect.SkipEntityAccessCheck)
            {
                return true;
            }

            return _entityAccessService.HasEntityAccess(EntityAccessTypes.Read,
                                                        EntityType.Instance.Account(),
                                                        _userContext.Identity.Code,
                                                        accountInfo.AccountId.Value,
                                                        accountInfo.OwnerCode.Value,
                                                        null);
        }
    }
}