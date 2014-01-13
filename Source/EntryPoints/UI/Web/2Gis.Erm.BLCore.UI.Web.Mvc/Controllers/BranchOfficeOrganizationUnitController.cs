using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{

    public sealed class BranchOfficeOrganizationUnitController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public BranchOfficeOrganizationUnitController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
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
            _publicService = publicService;
        }

        [HttpGet]
        public ActionResult SetAsPrimary()
        {
            return View(new SetBranchOfficeOrganizationUnitStatusViewModel());
        }

        [HttpPost]
        public ActionResult SetAsPrimary(SetBranchOfficeOrganizationUnitStatusViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
                return View(viewModel);

            try
            {
                _publicService.Handle(new SetBranchOfficeOrganizationUnitAsPrimaryRequest { Id = viewModel.Id });
                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }
            return View(viewModel);
        }

        public ActionResult SetAsPrimaryForRegSales()
        {
            return View(new SetBranchOfficeOrganizationUnitStatusViewModel());
        }

        [HttpPost]
        public ActionResult SetAsPrimaryForRegSales(SetBranchOfficeOrganizationUnitStatusViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                _publicService.Handle(new SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest { Id = viewModel.Id });
                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }
            return View(viewModel);
        }
    }
}