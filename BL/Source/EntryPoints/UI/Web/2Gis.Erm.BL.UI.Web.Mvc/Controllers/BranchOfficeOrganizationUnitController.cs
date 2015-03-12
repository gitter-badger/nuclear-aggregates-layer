using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{

    public sealed class BranchOfficeOrganizationUnitController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public BranchOfficeOrganizationUnitController(IMsCrmSettings msCrmSettings,
                                                      IAPIOperationsServiceSettings operationsServiceSettings,
                                                      IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                                      IIdentityServiceClientSettings identityServiceSettings,
                                                      IUserContext userContext,
                                                      ITracer tracer,
                                                      IGetBaseCurrencyService getBaseCurrencyService,
                                                      IPublicService publicService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
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
                ModelUtils.OnException(this, Tracer, viewModel, ex);
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
                ModelUtils.OnException(this, Tracer, viewModel, ex);
            }
            return View(viewModel);
        }
    }
}