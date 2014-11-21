using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AfterSaleServices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AfterSaleServiceController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationService _operationService;
        private readonly IPublicService _publicService;

        public AfterSaleServiceController(IMsCrmSettings msCrmSettings,
                                          IUserContext userContext,
                                          ICommonLog logger,
                                          ISecurityServiceFunctionalAccess functionalAccessService,
                                          IOperationService operationService,
                                          IPublicService publicService,
                                          IAPIOperationsServiceSettings operationsServiceSettings,
                                          IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                          IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _operationService = operationService;
            _publicService = publicService;
        }

        public ActionResult CreateAfterSaleServiceActivitiesDialog()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CreateAfterSalesServiceActivities, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.CreateAfterSalesActivitiesAccessDeniedError);
            }

            var model = new AfterSaleServiceCreateDialogModel
            {
                Month = DateTime.UtcNow.Date.GetFirstDateOfMonth(),
                UserId = UserContext.Identity.Code
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateAfterSaleServiceActivitiesDialog(AfterSaleServiceCreateDialogModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                var operationDescription = string.Format(BLResources.AfterSalesServiceOperationDescriptionFormat,
                                                         model.OrganizationUnit.Value,
                                                         model.Month.ToString("MMMM yyy"));

                var operation = new Operation
                    {
                        Description = operationDescription,
                        StartTime = DateTime.UtcNow,
                        Guid = Guid.NewGuid(),
                        OrganizationUnitId = model.OrganizationUnit.Key,
                        Status = OperationStatus.InProgress,
                        Type = BusinessOperation.AfterSaleServiceActivitiesCreation,
                    };
                _operationService.Add(operation);

                var period = new TimePeriod(model.Month.GetFirstDateOfMonth(), model.Month.GetEndPeriodOfThisMonth());

                CreateAfterSaleServiceActivities(model, model.OrganizationUnit.Key.Value, period, operation);
                _operationService.Update(operation);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, model, ex);
            }
            return View(model);
        }

        public ActionResult AfterSaleServiceOperationsJournal()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CreateAfterSalesServiceActivities, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.ViewAfterSalesActivitiesAccessDeniedError);
            }

            return RedirectToAction("View",
                                    "Grid",
                                    new
                                        {
                                            parentEntityType = EntityName.Operation,
                                            entityTypeName = EntityName.Operation,
                                            defaultDataView = "DListOperationsAfterSaleService"
                                        });
        }

        private void CreateAfterSaleServiceActivities(AfterSaleServiceCreateDialogModel model, long organizationUnitId, TimePeriod period, Operation operation)
        {
            var operationStatus = OperationStatus.Success;
            string operationAdditionalInfo;
            try
            {
                var ermResponse = (CreateAfterSaleServiceActivitiesResponse)
                    _publicService.Handle(new CreateAfterSaleServiceActivitiesRequest(organizationUnitId, period));

                var crmResponse = (CrmCreateAfterSaleServiceActivitiesResponse)
                    _publicService.Handle(new CrmCreateAfterSaleServiceActivitiesRequest(ermResponse.CreatedActivities));

                if (crmResponse.ErrorCount > 0)
                {
                    operationStatus = OperationStatus.Error;
                }

                operationAdditionalInfo = string.Format(BLResources.AfterSalesServiceOperationCompletedDetailsMessage,
                                                        ermResponse.DealsProcessed,
                                                        crmResponse.CreatedPhonecallsCount);

                if (crmResponse.ErrorCount > 0)
                {
                    operationAdditionalInfo += Environment.NewLine + string.Format(BLResources.AfterSalesServiceOperationErrorsMessage,
                                                                                   crmResponse.ErrorCount,
                                                                                   Environment.NewLine,
                                                                                   crmResponse.ErrorLog);
                }
            }
            catch (Exception ex)
            {
                operationStatus = OperationStatus.Error;
                operationAdditionalInfo = string.Format(BLResources.OperationCouldntCompleteMessage, ex.Message);
                ModelUtils.OnException(this, Logger, model, ex);
            }

            operation.FinishTime = DateTime.UtcNow;
            operation.OwnerCode = UserContext.Identity.Code;
            operation.Description = operation.Description + " " + operationAdditionalInfo;
            model.Message = operationAdditionalInfo;

            operation.Status = operationStatus;
        }
    }
}
