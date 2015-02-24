using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
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
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class WithdrawalInfoController : ControllerBase
    {
        private readonly IRevertWithdrawalOperationService _revertWithdrawalOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IWithdrawalsByAccountingMethodOperationService _withdrawalByAccountingMethodOperationService;
        private readonly IOperationService _operationService;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        public WithdrawalInfoController(IMsCrmSettings msCrmSettings,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                        IAPIIdentityServiceSettings identityServiceSettings,
                                        IUserContext userContext,
                                        ICommonLog logger,
                                        IGetBaseCurrencyService getBaseCurrencyService,
                                        IRevertWithdrawalOperationService revertWithdrawalOperationService,
                                        ISecurityServiceFunctionalAccess functionalAccessService,
                                        IWithdrawalsByAccountingMethodOperationService withdrawalByAccountingMethodOperationService,
                                        IOperationService operationService,
                                        IGlobalizationSettings globalizationSettings,
                                        IOrganizationUnitReadModel organizationUnitReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {

            _revertWithdrawalOperationService = revertWithdrawalOperationService;
            _functionalAccessService = functionalAccessService;
            _withdrawalByAccountingMethodOperationService = withdrawalByAccountingMethodOperationService;
            _operationService = operationService;
            _globalizationSettings = globalizationSettings;
            _organizationUnitReadModel = organizationUnitReadModel;
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
                var period = new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(),
                                            viewModel.PeriodStart.GetEndPeriodOfThisMonth());
                var processingResultsByOrganizationUnit =
                    _withdrawalByAccountingMethodOperationService.Withdraw(period, viewModel.AccountingMethod);

                var allWithwrawalsSucceded = processingResultsByOrganizationUnit.All(x => x.Value.Succeded);
                viewModel.IsSuccess = allWithwrawalsSucceded;

                if (!allWithwrawalsSucceded)
                {
                    var operationId = Guid.NewGuid();
                    var resultMessage = string.Format(BLResources.WithdrawalFailed,
                                                             period.Start,
                                                             period.End,
                                                             viewModel.AccountingMethod.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));

                    var operation = new Operation
                                        {
                                            Guid = operationId,
                                            StartTime = DateTime.UtcNow,
                                            FinishTime = DateTime.UtcNow,
                                            OwnerCode = UserContext.Identity.Code,
                                            Status = OperationStatus.Error,
                                            Type = BusinessOperation.Withdrawal,
                                            Description = resultMessage,
                                        };

                    var report = GetErrorsReport(processingResultsByOrganizationUnit.Where(x => !x.Value.Succeded).ToDictionary(x => x.Key, y => y.Value),
                                                 period,
                                                 viewModel.AccountingMethod);

                    _operationService.FinishOperation(operation,
                                                      report.ReportContent,
                                                      HttpUtility.UrlPathEncode(report.ReportFileName),
                                                      report.ContentType);
                    viewModel.HasErrors = true;
                    viewModel.ErrorLogFileId = operationId;
                    viewModel.Message = resultMessage;
                }
                else
                {
                    viewModel.Message = "Withdrawal successfully finished";
                } 
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
                            viewModel.AccountingMethod,
                            viewModel.Comment);

                viewModel.Message = processingResult.Succeded
                                        ? "Withdrawal successfully reverted"
                                        : processingResult.ProcessingMessages.Aggregate(new StringBuilder(), (builder, message) => builder.AppendLine(message.Text)).ToString();
                viewModel.IsSuccess = processingResult.Succeded;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        #endregion

        private ErrorsReport GetErrorsReport(IDictionary<long, WithdrawalProcessingResult> resultsWithErrors, TimePeriod period, AccountingMethod accountingMethod)
        {
            var organizationUnitNames = _organizationUnitReadModel.GetNames(resultsWithErrors.Select(x => x.Key));
            var dataTable = new DataTable();
            dataTable.Columns.Add(MetadataResources.OrganizationUnit);
            dataTable.Columns.Add(BLResources.Error);

            foreach (var withdrawalProcessingResult in resultsWithErrors)
            {
                foreach (var message in withdrawalProcessingResult.Value.ProcessingMessages)
                {
                    dataTable.Rows.Add(organizationUnitNames[withdrawalProcessingResult.Key], message.Text);
                }
            }

            var csvReportContent = dataTable.ToCsvEscaped(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator, true);
            var reportContent = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvReportContent)).ToArray();

            return new ErrorsReport
                       {
                           ReportContent = reportContent,
                           ReportFileName = string.Format("WithdrawalReport{0:dd-MM-yy}_{1:dd-MM-yy}_{2}.csv", period.Start, period.End, accountingMethod),
                           ContentType = "text/csv",
                       };
        }

        private class ErrorsReport
        {
            public byte[] ReportContent { get; set; }
            public string ReportFileName { get; set; }
            public string ContentType { get; set; }
        }
    }
}
