using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public class OrderDialogsController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly IUserRepository _userRepository;
        private readonly IOperationService _operationService;

        public OrderDialogsController(IMsCrmSettings msCrmSettings,
                                      IUserContext userContext,
                                      ICommonLog logger,
                                      IPublicService publicService,
                                      IUserRepository userRepository,
                                      IOperationService operationService,
                                      IAPIOperationsServiceSettings operationsServiceSettings,
                                      IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
            _publicService = publicService;
            _userRepository = userRepository;
            _operationService = operationService;
        }

        [HttpGet]
        public ActionResult MakeRegionalAdsDocs()
        {
            var currentUserId = UserContext.Identity.Code;

            var model = new MakeRegionalAdsDocsViewModel
                            {
                                StartPeriodDate = DateTime.Now.AddMonths(1).GetFirstDateOfMonth(),
                                UserId = currentUserId
                            };

            OrganizationUnit orgUnit;
            if (_userRepository.TryGetSingleUserOrganizationUnit(currentUserId, out orgUnit))
            {
                model.SourceOrganizationUnit = new LookupField
                                                                                                                     {
                                                       Key = orgUnit.Id,
                                                       Value = orgUnit.Name
                                                   };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult MakeRegionalAdsDocs(MakeRegionalAdsDocsViewModel viewModel)
        {
            viewModel.ResultOperationGuid = null;
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            if (viewModel.SourceOrganizationUnit.Key == null)
            {
                throw new ArgumentException();
            }

            try
            {
                var response = (StreamResponse)_publicService.Handle(new MakeRegionalAdsDocsRequest
                {
                    StartPeriodDate = viewModel.StartPeriodDate,
                    SourceOrganizationUnitId = (long)viewModel.SourceOrganizationUnit.Key
                });

                var operationId = Guid.NewGuid();
                var operationDescription = string.Format(BLResources.RegionalAdsDocsDescriptionTemplate, viewModel.SourceOrganizationUnit.Value, viewModel.StartPeriodDate.ToString("MMMM yyy"));

                var operation = new Operation
                {
                    Guid = operationId,
                    StartTime = DateTime.UtcNow,
                    FinishTime = DateTime.UtcNow,
                    OwnerCode = UserContext.Identity.Code,
                    Status = (byte)OperationStatus.Success,
                    Type = (short)BusinessOperation.MakeRegionalAdsDocs,
                    Description = operationDescription,
                    OrganizationUnitId = viewModel.SourceOrganizationUnit.Key
                };

                // Записываем результат в базу, ид результата передаем в форму.
                _operationService.FinishOperation(operation, response.Stream, response.FileName, response.ContentType);
                viewModel.ResultOperationGuid = operationId;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }
    }
}
