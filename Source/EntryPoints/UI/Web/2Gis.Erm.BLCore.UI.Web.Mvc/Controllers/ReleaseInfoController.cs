using System;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.API.Releasing.Releases.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
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
    public sealed class ReleaseInfoController : ControllerBase
    {
        private readonly IStartSimplifiedReleaseOperationService _startSimplifiedReleaseOperationService;
        private readonly IFinishReleaseOperationService _finishReleaseOperationService;
        private readonly IRevertReleaseOperationService _revertReleaseOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;

        public ReleaseInfoController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IStartSimplifiedReleaseOperationService startSimplifiedReleaseOperationService,
            IFinishReleaseOperationService finishReleaseOperationService,
            IRevertReleaseOperationService revertReleaseOperationService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IPublicService publicService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _startSimplifiedReleaseOperationService = startSimplifiedReleaseOperationService;
            _finishReleaseOperationService = finishReleaseOperationService;
            _revertReleaseOperationService = revertReleaseOperationService;
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
        }

        [HttpGet]
        public ActionResult DownloadResults(long id)
        {
            var response = (StreamResponse)_publicService.Handle(new DownloadReleaseInfoResultsRequest { ReleaseInfoId = id });
            if (response.Stream == null)
            {
                return HttpNotFound(string.Format(BLResources.ReleaseValidationResultsNotFound, id));
            }

            return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
        }

        #region execute release

        public ActionResult ReleaseDialog()
        {
            var model = new ReleaseDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.AddMonths(1).GetFirstDateOfMonth()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ReleaseDialog(ReleaseDialogViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                var releaseDescriptor = 
                    _startSimplifiedReleaseOperationService.Start(
                            viewModel.OrganizationUnit.Key.Value,
                            new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()),
                            viewModel.IsBeta);

                _finishReleaseOperationService.Succeeded(releaseDescriptor.ReleaseId);

                viewModel.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                viewModel.SetWarning(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        #endregion

        #region revert release

        public ActionResult ReleaseRevertDialog()
        {
            var model = new ReleaseRevertDialogViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.AddMonths(1).GetFirstDateOfMonth()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ReleaseRevertDialog(ReleaseRevertDialogViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                _revertReleaseOperationService.Revert(
                    viewModel.OrganizationUnit.Key.Value, 
                    new TimePeriod(viewModel.PeriodStart.GetFirstDateOfMonth(), viewModel.PeriodStart.GetEndPeriodOfThisMonth()), 
                    viewModel.Comment);

                viewModel.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                viewModel.SetWarning(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
