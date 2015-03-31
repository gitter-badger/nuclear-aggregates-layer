using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class LimitViewModelCustomizationService : IGenericViewModelCustomizationService<Limit>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IAccountReadModel _accountReadModel;

        public LimitViewModelCustomizationService(IUserContext userContext,
                                                  ISecurityServiceUserIdentifier userIdentifierService,
                                                  ISecurityServiceFunctionalAccess functionalAccessService,
                                                  IAccountReadModel accountReadModel)
        {
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _accountReadModel = accountReadModel;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LimitViewModel)viewModel;

            if (entityViewModel.Inspector != null)
            {
                entityViewModel.Inspector.Value = _userIdentifierService.GetUserInfo(entityViewModel.Inspector.Key).DisplayName;
            }

            if (entityViewModel.Status != LimitStatus.Opened)
            {
                entityViewModel.ViewConfig.ReadOnly = true;
            }

            entityViewModel.HasEditPeriodPrivelege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitPeriodChanging,
                                                                                                            _userContext.Identity.Code);

            var toolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar.ToArray();

            if (entityViewModel.IsNew)
            {
                DisableButtons(toolbar, new[] { "OpenLimit", "RejectLimit", "ApproveLimit", "RecalculateLimit", "IncreaseLimit" });
                return;
            }

            switch (entityViewModel.Status)
            {
                case LimitStatus.Opened:
                    DisableButtons(toolbar, new[] { "OpenLimit", "IncreaseLimit" });
                    break;
                case LimitStatus.Approved:
                     DisableButtons(toolbar, new[] { "RejectLimit", "ApproveLimit" });
                    break;
                case LimitStatus.Rejected:
                    DisableButtons(toolbar, new[] { "RejectLimit", "ApproveLimit", "IncreaseLimit" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("model");
            }

            // Кнопка "Пересчитать" в карточке лимита должна быть доступна с момента создания лимита и до тех пор пока за период по которому выставлен лимит отсутствует финальная,
            // успешная сборка по городам назначения заказов входящих в расчёт суммы лимита.
            if (!_accountReadModel.IsLimitRecalculationAvailable(entityViewModel.Id))
            {
                DisableButtons(toolbar, new[] { "RecalculateLimit" });
            }
        }

        private static void DisableButtons(IEnumerable<ToolbarJson> toolbar, IEnumerable<string> statusButtons)
        {
            var buttonsToDisable = toolbar.Where(item => statusButtons.Contains(item.Name, StringComparer.OrdinalIgnoreCase));
            foreach (var item in buttonsToDisable)
            {
                item.Disabled = true;
            }
        }
    }
}