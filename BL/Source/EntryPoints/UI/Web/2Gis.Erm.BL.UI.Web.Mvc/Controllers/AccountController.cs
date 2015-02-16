using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AccountController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly IOperationService _operationService;
        private readonly IFinder _finder;

        public AccountController(IMsCrmSettings msCrmSettings,
                                 IAPIOperationsServiceSettings operationsServiceSettings,
                                 IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                 IAPIIdentityServiceSettings identityServiceSettings,
                                 IUserContext userContext,
                                 ICommonLog logger,
                                 IGetBaseCurrencyService getBaseCurrencyService,
                                 ISecurityServiceFunctionalAccess functionalAccessService,
                                 IPublicService publicService,
                                 IOperationService operationService,
                                 IFinder finder)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _operationService = operationService;
            _finder = finder;
        }

        public ActionResult ExportTo1CDialog()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.FranchiseesWithdrawalExport, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }
        
            var currentIdentity = UserContext.Identity;
            var organizationUnits =
                _finder.Find<OrganizationUnit>(
                            x => x.UserTerritoriesOrganizationUnits.Any(y => y.UserId == currentIdentity.Code) 
                            && x.IsActive 
                            && !x.IsDeleted 
                            && x.ErmLaunchDate != null 
                            && x.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary).BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees)
                        .Select(x => new
                                  {
                                      x.Id,
                                      x.Name
                                  }).ToArray();

            var organizationUnitsCount = organizationUnits.Length;
        
            var model = new ExportAccountTo1CViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.GetNextMonthFirstDate(),
                OrganizationUnit = new LookupField
                {
                    Key = organizationUnitsCount == 1 ? organizationUnits[0].Id : (long?)null,
                    Value = organizationUnitsCount == 1 ? organizationUnits[0].Name : null
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ExportTo1CDialog(ExportAccountTo1CViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                var response = (IntegrationResponse)_publicService.Handle(new ExportLocalMessageRequest
                {
                    IntegrationType = IntegrationTypeExport.AccountDetailsTo1C,
                    OrganizationUnitId = viewModel.OrganizationUnit.Key.Value,
                    PeriodStart = viewModel.PeriodStart,
                });

                if (response.BlockingErrorsAmount != 0)
                {
                    viewModel.SetCriticalError(BLResources.ExportFailed);
                }
                else
                {
                    viewModel.Message = BLResources.ExportSucceeded;
                    viewModel.HasResult = true;

                    var operationId = Guid.NewGuid();

                    var operation = new Operation
                        {
                            Guid = operationId,
                            StartTime = DateTime.UtcNow,
                            FinishTime = DateTime.UtcNow,
                            OwnerCode = UserContext.Identity.Code,
                            Status = OperationStatus.Success,
                            Type = BusinessOperation.ExportAccountDetailsTo1CForFranchisees,
                            Description = BLResources.ExportSucceeded,
                            OrganizationUnitId = viewModel.OrganizationUnit.Key
                        };

                    _operationService.FinishOperation(operation, response.Stream, HttpUtility.UrlPathEncode(response.FileName), response.ContentType);
                    viewModel.FileId = operationId;
                }
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        public ActionResult ExportToServiceBusDialog()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.FranchiseesWithdrawalExport, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            var currentIdentity = UserContext.Identity;
            var organizationUnits =
                _finder.Find<OrganizationUnit>(
                            x => x.UserTerritoriesOrganizationUnits.Any(y => y.UserId == currentIdentity.Code)
                            && x.IsActive
                            && !x.IsDeleted
                            && x.ErmLaunchDate != null
                            && x.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary).BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees)
                        .Select(x => new
                        {
                            x.Id,
                            x.Name
                        }).ToArray();

            var organizationUnitsCount = organizationUnits.Length;

            var model = new ExportAccountToServiceBusViewModel
            {
                PeriodStart = DateTime.UtcNow.Date.GetNextMonthFirstDate(),
                OrganizationUnit = new LookupField
                {
                    Key = organizationUnitsCount == 1 ? organizationUnits[0].Id : (long?)null,
                    Value = organizationUnitsCount == 1 ? organizationUnits[0].Name : null
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ExportToServiceBusDialog(ExportAccountToServiceBusViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                var response = (IntegrationResponse)_publicService.Handle(new ExportLocalMessageRequest
                {
                    IntegrationType = IntegrationTypeExport.AccountDetailsToServiceBus,
                    OrganizationUnitId = viewModel.OrganizationUnit.Key.Value,
                    PeriodStart = viewModel.PeriodStart,
                    CreateCsvFile = viewModel.CreateCsvFile
                });

                if (response.BlockingErrorsAmount != 0)
                {
                    viewModel.SetCriticalError(BLResources.ExportFailed);
                }
                else
                {
                    viewModel.Message = BLResources.ExportSucceeded;
                    viewModel.HasResult = true;

                    var operationId = Guid.NewGuid();

                    var operation = new Operation
                    {
                        Guid = operationId,
                        StartTime = DateTime.UtcNow,
                        FinishTime = DateTime.UtcNow,
                        OwnerCode = UserContext.Identity.Code,
                        Status = OperationStatus.Success,
                        Type = BusinessOperation.ExportAccountDetailsTo1CForFranchisees,
                        Description = BLResources.ExportSucceeded,
                        OrganizationUnitId = viewModel.OrganizationUnit.Key
                    };

                    _operationService.FinishOperation(operation, response.Stream, HttpUtility.UrlPathEncode(response.FileName), response.ContentType);
                    viewModel.FileId = operationId;
                }
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }
    }
}
