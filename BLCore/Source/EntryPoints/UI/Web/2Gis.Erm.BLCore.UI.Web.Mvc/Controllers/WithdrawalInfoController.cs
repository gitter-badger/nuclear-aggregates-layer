using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class WithdrawalInfoController : ControllerBase
    {
        private readonly IWithdrawalOperationService _withdrawalOperationService;
        private readonly IRevertWithdrawalOperationService _revertWithdrawalOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public WithdrawalInfoController(IMsCrmSettings msCrmSettings,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                        IAPIIdentityServiceSettings identityServiceSettings,
                                        IUserContext userContext,
                                        ITracer tracer,
                                        IGetBaseCurrencyService getBaseCurrencyService,
                                        IWithdrawalOperationService withdrawalOperationService,
                                        IRevertWithdrawalOperationService revertWithdrawalOperationService,
                                        ISecurityServiceFunctionalAccess functionalAccessService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _withdrawalOperationService = withdrawalOperationService;
            _revertWithdrawalOperationService = revertWithdrawalOperationService;
            _functionalAccessService = functionalAccessService;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }
            base.OnActionExecuting(filterContext);
        }

        #region execute withdrawal

        public ActionResult WithdrawalDialog()
        {
            var model = new WithdrawalDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.GetNextMonthFirstDate()
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
                var processingResult =
                    _withdrawalOperationService.Withdraw(viewModel.OrganizationUnit.Key.Value,
                                                         new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()));

                viewModel.Message = processingResult.Succeded
                                        ? "Withdrawal successfully finished"
                                        : processingResult.ProcessingMessages.Aggregate(new StringBuilder(), (builder, message) => builder.AppendLine(message.Text)).ToString();
                viewModel.IsSuccess = processingResult.Succeded;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, viewModel, ex);
            }

            return View(viewModel);
        }

        #endregion

        #region revert withdrawal

        public ActionResult WithdrawalRevertDialog()
        {
            var model = new WithdrawalRevertDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.GetNextMonthFirstDate()
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
                var processingResult = 
                    _revertWithdrawalOperationService.Revert(
                            viewModel.OrganizationUnit.Key.Value,
                            new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()),
                            viewModel.Comment);

                viewModel.Message = processingResult.Succeded
                                        ? "Withdrawal successfully reverted"
                                        : processingResult.ProcessingMessages.Aggregate(new StringBuilder(), (builder, message) => builder.AppendLine(message.Text)).ToString();
                viewModel.IsSuccess = processingResult.Succeded;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, viewModel, ex);
            }

            return View(viewModel);
        }

        #endregion
    }
}
