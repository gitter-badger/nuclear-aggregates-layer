using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Limits
{
    public sealed class CheckIfLimitRecalculationAvailableCustomization : IViewModelCustomization<LimitViewModel>
    {
        private readonly IAccountRepository _accountRepository;

        public CheckIfLimitRecalculationAvailableCustomization(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Customize(LimitViewModel viewModel, ModelStateDictionary modelState)
        {
            // Кнопка "Пересчитать" в карточке лимита должна быть доступна с момента создания лимита и до тех пор пока за период по которому выставлен лимит отсутствует финальная,
            // успешная сборка по городам назначения заказов входящих в расчёт суммы лимита.
            if (!_accountRepository.IsLimitRecalculationAvailable(viewModel.Id))
            {
                viewModel.ViewConfig.DisableCardToolbarItem("RecalculateLimit");
            }
        }
    }
}