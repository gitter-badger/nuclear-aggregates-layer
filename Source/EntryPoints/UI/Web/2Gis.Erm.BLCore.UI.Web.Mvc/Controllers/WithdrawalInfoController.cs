using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class WithdrawalInfoController : ControllerBase
    {
        private readonly IWithdrawalOperationService _withdrawalOperationService;
        private readonly IRevertWithdrawalOperationService _revertWithdrawalOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public WithdrawalInfoController(
            IWithdrawalOperationService withdrawalOperationService,
            IRevertWithdrawalOperationService revertWithdrawalOperationService,
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _withdrawalOperationService = withdrawalOperationService;
            _revertWithdrawalOperationService = revertWithdrawalOperationService;
            _functionalAccessService = functionalAccessService;
        }

        #region card
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }
            base.OnActionExecuting(filterContext);
        }

        #endregion

        #region execute withdrawal

        public ActionResult WithdrawalDialog()
        {
            var model = new WithdrawalDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.AddMonths(1).GetFirstDateOfMonth()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult WithdrawalDialog(WithdrawalDialogViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }
            try
            {
                _withdrawalOperationService.Withdraw(
                    viewModel.OrganizationUnit.Key.Value, 
                    new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()));
                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }
            return View(viewModel);
        }

        #endregion

        #region revert withdrawal

        public ActionResult WithdrawalRevertDialog()
        {
            var model = new WithdrawalRevertDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.AddMonths(1).GetFirstDateOfMonth()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult WithdrawalRevertDialog(WithdrawalRevertDialogViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
                return View(viewModel);

            try
            {
                _revertWithdrawalOperationService.Revert(
                    viewModel.OrganizationUnit.Key.Value,
                    new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()),
                    viewModel.Comment);
                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }
            return View(viewModel);
        }

        #endregion
    }
}
