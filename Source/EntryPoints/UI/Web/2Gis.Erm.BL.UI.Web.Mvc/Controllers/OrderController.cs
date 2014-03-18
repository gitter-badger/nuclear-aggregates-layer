using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
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
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IReplicationCodeConverter _replicationCodeConverter;
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _secureFinder;

        private readonly IFinder _finder;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderRepository _orderRepository;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationService _operationService;

        private readonly IProcessOrderProlongationRequestSingleOperation _orderProlongationOperation;
        private readonly IProcessOrderCreationRequestSingleOperation _orderCreationOperation;
        private readonly ICopyOrderOperationService _copyOrderOperationService;
        private readonly IRepairOutdatedPositionsOperationService _repairOutdatedPositionsOperationService;

        public OrderController(IMsCrmSettings msCrmSettings,
                               IUserContext userContext,
                               ICommonLog logger,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               ISecurityServiceUserIdentifier userIdentifierService,
                               ISecurityServiceFunctionalAccess functionalAccessService,
                               IReplicationCodeConverter replicationCodeConverter,
                               IPublicService publicService,
                               ISecureFinder secureFinder,
                               IFinder finder,
                               IReleaseReadModel releaseReadModel,
                               IBranchOfficeRepository branchOfficeRepository,
                               IOrderReadModel orderReadModel,
                               IOrderRepository orderRepository,
                               ILegalPersonRepository legalPersonRepository,
                               IOperationService operationService,
                               IProcessOrderProlongationRequestSingleOperation orderProlongationOperation,
                               IProcessOrderCreationRequestSingleOperation orderCreationOperation,
                               ICopyOrderOperationService copyOrderOperationService,
                               IRepairOutdatedPositionsOperationService repairOutdatedPositionsOperationService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _replicationCodeConverter = replicationCodeConverter;
            _publicService = publicService;
            _secureFinder = secureFinder;
            _finder = finder;
            _releaseReadModel = releaseReadModel;
            _branchOfficeRepository = branchOfficeRepository;
            _orderReadModel = orderReadModel;
            _orderRepository = orderRepository;
            _legalPersonRepository = legalPersonRepository;
            _operationService = operationService;
            _orderProlongationOperation = orderProlongationOperation;
            _orderCreationOperation = orderCreationOperation;
            _copyOrderOperationService = copyOrderOperationService;
            _repairOutdatedPositionsOperationService = repairOutdatedPositionsOperationService;
        }



        #region Ajax methods

        [HttpPost]
        public JsonNetResult GetReleasesNumbers(long? organizationUnitid, DateTime? beginDistributionDate, int? releaseCountPlan)
        {
            if (!organizationUnitid.HasValue || !beginDistributionDate.HasValue || !releaseCountPlan.HasValue)
            {
                return new JsonNetResult();
            }

            var releaseNumbersDto = _orderReadModel.CalculateReleaseNumbers(organizationUnitid.Value, beginDistributionDate.Value, releaseCountPlan.Value, releaseCountPlan.Value);
            var distributionDatesDto = _orderReadModel.CalculateDistributionDates(beginDistributionDate.Value, releaseCountPlan.Value, releaseCountPlan.Value);

            return new JsonNetResult(new
            {
                releaseNumbersDto.BeginReleaseNumber,
                releaseNumbersDto.EndReleaseNumberPlan,
                releaseNumbersDto.EndReleaseNumberFact,

                distributionDatesDto.BeginDistributionDate,
                distributionDatesDto.EndDistributionDatePlan,
                distributionDatesDto.EndDistributionDateFact,
            });
        }

        [HttpPost]
        public JsonNetResult GetCurrency(long? organizationUnitid)
        {
            if (!organizationUnitid.HasValue)
            {
                return new JsonNetResult();
            }

            var currencyInfo = _secureFinder.Find<OrganizationUnit>(x => !x.IsDeleted && x.IsActive && x.Id == organizationUnitid.Value)
                .Select(x => new { x.Country.Currency.Id, x.Country.Currency.Name }).FirstOrDefault();

            return (currencyInfo == null)
                       ? new JsonNetResult()
                       : new JsonNetResult(new { currencyInfo.Id, currencyInfo.Name });
        }

        [HttpPost]
        public JsonNetResult GetBranchOfficeOrganizationUnit(long? organizationUnitid)
        {
            if (!organizationUnitid.HasValue)
            {
                return new JsonNetResult();
            }

            var branchOfficeOrganizationUnit = _branchOfficeRepository.GetBranchOfficeOrganizationUnitShortInfo(organizationUnitid.Value);
            return new JsonNetResult(new { Id = branchOfficeOrganizationUnit.Id, Name = branchOfficeOrganizationUnit.ShortLegalName });
        }

        [HttpPost]
        public JsonNetResult GetLegalPerson(long? firmClientId)
        {
            if (!firmClientId.HasValue)
            {
                return new JsonNetResult();
            }

            var resp = (GetOrderLegalPersonResponse)_publicService.Handle(new GetOrderLegalPersonRequest { FirmClientId = firmClientId.Value });
            return new JsonNetResult(new { Id = resp.LegalPersonId, Name = resp.LegalPersonName });
        }

        public JsonNetResult GetHasDestOrganizationUnitPublishedPrice(long? orderId, long? orgUnitId)
        {
            if (!orderId.HasValue || !orgUnitId.HasValue)
            {
                return new JsonNetResult();
            }

            var beginDistributionDate = _secureFinder.Find<Order>(o => o.Id == orderId.Value).Select(o => o.BeginDistributionDate).FirstOrDefault();

            var hasDestOrganizationUnitPublishedPrice =
                _secureFinder.Find<OrganizationUnit>(ou => ou.Id == orgUnitId.Value)
                       .Any(ou => ou.Prices.Any(price => price.IsPublished && price.IsActive && !price.IsDeleted && price.BeginDate <= beginDistributionDate));

            return new JsonNetResult(hasDestOrganizationUnitPublishedPrice);
        }

        [HttpPost]
        public JsonNetResult GetDestinationOrganizationUnit(long? firmId)
        {
            if (!firmId.HasValue)
            {
                return new JsonNetResult();
            }

            var resp = (GetOrderDestinationOrganizationUnitResponse)_publicService.Handle(new GetOrderDestinationOrganizationUnitRequest { FirmId = firmId.Value });
            return new JsonNetResult(new { Id = resp.OrganizationUnitId, Name = resp.OrganizationUnitName });
        }

        [HttpPost]
        public JsonNetResult DiscountRecalc(RecalculateOrderDiscountRequest request)
        {
            try
            {
                var response = (RecalculateOrderDiscountResponse)_publicService.Handle(request);
                return new JsonNetResult(response);
            }
            catch (Exception ex)
            {
                var model = new ViewModel();
                ModelUtils.OnException(this, Logger, model, ex);
                return new JsonNetResult(new { model.Message, model.MessageType });
            }
        }

        // TODO {d.ivanov, y.baranihin, 25.02.2013}: Учесть возможность явного получения текущего состояния агрегата, без использования скрытых полей типа SerializeSteObject
        [HttpGet]
        public JsonNetResult GetOrderAggregateInCurrentState(long id)
        {
            var orderAggregate = _secureFinder.Find<Order>(x => x.Id == id)
                .Select(x => new
                {
                    Order = x,
                    Platform = x.Platform.Name,
                    DiscountReason = (OrderDiscountReason)x.DiscountReasonEnum,
                    BudgetType = (OrderBudgetType)x.BudgetType,
                    DiscountInPercents = x.OrderPositions
                                .Where(y => !y.IsDeleted && y.IsActive)
                                                                            .All(y => y.CalculateDiscountViaPercent),
                    x.Timestamp
                })
                .AsEnumerable()
                .Select(x => new
                {
                    Order = x.Order,
                    Platform = x.Platform,
                    DiscountReason = x.DiscountReason,
                    BudgetType = x.BudgetType,
                    DiscountInPercents = x.DiscountInPercents,
                    EntityStateToken = Convert.ToBase64String(x.Timestamp)
                })
                .Single();

            return new JsonNetResult(orderAggregate);
        }

        [HttpGet]
        public JsonNetResult CanCreateOrderPositionsForOrder(long orderId, string orderTypeValue)
        {
            CanCreateOrderPositionForOrderResponse response;
            OrderType orderType;
            if (!Enum.TryParse(orderTypeValue, out orderType))
            {
                response = new CanCreateOrderPositionForOrderResponse { Message = BLResources.WrongOrderType };
            }
            else
            {
                response = (CanCreateOrderPositionForOrderResponse)_publicService.Handle(new CanCreateOrderPositionForOrderRequest
                {
                    OrderId = orderId,
                    OrderType = orderType
                });
            }

            return new JsonNetResult(response);
        }

        [HttpPost]
        public JsonNetResult CheckBeginDistributionDate(long orderId, DateTime beginDistributionDate, long sourceOrganizationUnitId, long destinationOrganizationUnitId)
        {
            _publicService.Handle(new CheckOrderBeginDistributionDateRequest
            {
                OrderId = orderId,
                BeginDistributionDate = beginDistributionDate,
                SourceOrganizationUnitId = sourceOrganizationUnitId,
                DestinationOrganizationUnitId = destinationOrganizationUnitId
            });
            return new JsonNetResult();
        }

        #endregion

        public ActionResult CheckOrdersReadinessForReleaseDialog()
        {
            var currentUser = UserContext.Identity;
            var nextMonth = DateTime.Today.AddMonths(1);

            var firstUserOrgUnit = _finder.Find(Specs.Find.ById<User>(currentUser.Code))
                                          .SelectMany(user => user.UserOrganizationUnits)
                                          .Select(unit => new { unit.OrganizationUnitDto.Id, unit.OrganizationUnitDto.Name })
                                          .FirstOrDefault();

            var model = new CheckOrdersReadinessForReleaseDialogViewModel
            {
                StartPeriodDate = nextMonth.GetFirstDateOfMonth(),
                Owner = new LookupField
                {
                    Key = currentUser.Code,
                    Value = _userIdentifierService.GetUserInfo(currentUser.Code).DisplayName
                },
                OrganizationUnit = new LookupField
                {
                    Key = firstUserOrgUnit != null ? firstUserOrgUnit.Id : (long?)null,
                    Value = firstUserOrgUnit != null ? firstUserOrgUnit.Name : null
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckOrdersReadinessForReleaseDialog(CheckOrdersReadinessForReleaseDialogViewModel viewModel)
        {
            try
            {
                if (viewModel.OrganizationUnit == null || !viewModel.OrganizationUnit.Key.HasValue)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OrganizationUnit));
                }

                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PrereleaseOrderValidationExecution, UserContext.Identity.Code))
                {
                    throw new NotificationException(BLResources.AccessDeniedPrereleaseOrderValidationExecution);
                }

                var period = new TimePeriod(viewModel.StartPeriodDate, viewModel.StartPeriodDate.GetEndPeriodOfThisMonth());

                if (_releaseReadModel.HasFinalReleaseInProgress(viewModel.OrganizationUnit.Key.Value, period))
                {
                    throw new NotificationException(string.Format(BLResources.ReleaseIsInProgressForOrganizationUnit, viewModel.OrganizationUnit.Value));
                }

                var response = (CheckOrdersReadinessForReleaseResponse)_publicService.Handle(new CheckOrdersReadinessForReleaseRequest
                {
                    OrganizationUnitId = viewModel.OrganizationUnit.Key,
                    OwnerId = viewModel.OrganizationUnit != null ? viewModel.Owner.Key : null,
                    IncludeOwnerDescendants = viewModel.IncludeOwnerDescendants,
                    CheckAccountBalance = viewModel.CheckAccountBalance,
                    Period = period
                });

                viewModel.Message = BLResources.CheckingIsFinished;
                if (response.HasErrors)
                {
                    viewModel.Message = response.Message;
                    viewModel.HasErrors = true;

                    var operationId = Guid.NewGuid();
                    var operationDescription = string.Format(BLResources.CheckingOrdersHasFinishedWithErrors, viewModel.OrganizationUnit.Value, viewModel.StartPeriodDate.ToString("MMMM yyy"));

                    var operation = new Operation
                    {
                        Guid = operationId,
                        StartTime = DateTime.UtcNow,
                        FinishTime = DateTime.UtcNow,
                        OwnerCode = UserContext.Identity.Code,
                        Status = (byte)OperationStatus.Error,
                        Type = (short)BusinessOperation.CheckOrdersReadinessForRelease,
                        Description = operationDescription,
                        OrganizationUnitId = viewModel.OrganizationUnit.Key
                    };

                    _operationService.FinishOperation(operation, response.ReportContent, HttpUtility.UrlPathEncode(response.ReportFileName), response.ContentType);
                    viewModel.ErrorLogFileId = operationId;
                }
                else
                {
                    viewModel.Message = response.Message;
                    viewModel.HasErrors = false;
                }
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        public ActionResult GetOrdersWithDummyAdvertisementDialog()
        {
            var currentUser = UserContext.Identity;

            var firstUserOrgUnit = _finder.Find(Specs.Find.ById<User>(currentUser.Code))
                                          .SelectMany(user => user.UserOrganizationUnits)
                                          .Select(unit => new { unit.OrganizationUnitDto.Id, unit.OrganizationUnitDto.Name })
                                          .FirstOrDefault();

            var model = new GetOrdersWithDummyAdvertisementDialogModel
            {
                Owner = new LookupField
                {
                    Key = currentUser.Code,
                    Value = _userIdentifierService.GetUserInfo(currentUser.Code).DisplayName
                },
                OrganizationUnit = new LookupField
                {
                    Key = firstUserOrgUnit != null ? firstUserOrgUnit.Id : (long?)null,
                    Value = firstUserOrgUnit != null ? firstUserOrgUnit.Name : null
                },
                UserId = currentUser.Code
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetOrdersWithDummyAdvertisementDialog(GetOrdersWithDummyAdvertisementDialogModel viewModel)
        {
            try
            {
                if (viewModel.OrganizationUnit == null || !viewModel.OrganizationUnit.Key.HasValue)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.OrganizationUnit));
                }

                if (viewModel.Owner == null || !viewModel.Owner.Key.HasValue)
                {
                    throw new NotificationException(string.Format(BLResources.RequiredFieldMessage, MetadataResources.Owner));
                }

                var response = (GetOrdersWithDummyAdvertisementResponse)_publicService.Handle(new GetOrdersWithDummyAdvertisementRequest
                {
                    OrganizationUnitId = viewModel.OrganizationUnit.Key.Value,
                    OwnerId = viewModel.Owner.Key.Value,
                    IncludeOwnerDescendants = viewModel.IncludeOwnerDescendants,
                });

                viewModel.Message = BLResources.CheckingIsFinished;
                if (response.HasOrders)
                {
                    viewModel.Message = response.Message;
                    viewModel.HasOrders = true;

                    var operationId = Guid.NewGuid();
                    var operationDescription = string.Format("Поиск заказов с заглушками РМ по {0} успешно завершен", viewModel.OrganizationUnit.Value);

                    var operation = new Operation
                    {
                        Guid = operationId,
                        StartTime = DateTime.UtcNow,
                        FinishTime = DateTime.UtcNow,
                        OwnerCode = UserContext.Identity.Code,
                        Status = (byte)OperationStatus.Success,
                        Type = (short)OldBusinessOperationType.GetOrdersWithDummyAdvertisements,
                        Description = operationDescription,
                        OrganizationUnitId = viewModel.OrganizationUnit.Key
                    };

                    _operationService.FinishOperation(
                        operation,
                        response.ReportContent,
                        HttpUtility.UrlPathEncode(response.ReportFileName),
                        response.ContentType);

                    viewModel.OrdersListFileId = operationId;
                }
                else
                {
                    viewModel.Message = response.Message;
                    viewModel.HasOrders = false;
                }
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult RemoveBargain(long orderId)
        {
            _publicService.Handle(new RemoveBargainFromOrderRequest { OrderId = orderId });
            return null;
        }

        [HttpPost]
        public JsonNetResult GetBargainRemovalConfirmation(long orderId)
        {
            var hasAnotherOrders = _secureFinder.Find<Order>(order => order.Id == orderId)
                                                                        .Select(order => order.Bargain.Orders.Any(item => item.Id != orderId && !item.IsDeleted))
                                                                        .Single();
            return new JsonNetResult(hasAnotherOrders
                       ? BLResources.RemoveOrderBargainLinkConfirmation
                       : BLResources.RemoveBargainConfirmation);
        }

        [HttpGet]
        public ActionResult ChangeOrderDeal(long orderId)
        {
            var model = new ChangeOrderDealViewModel();
            if (orderId != 0)
            {
                var dealInfo = _secureFinder.Find<Order>(x => x.Id == orderId)
                    .Select(x => new { Id = (long?)x.Deal.Id, x.Deal.Name })
                    .SingleOrDefault();
                if (dealInfo != null)
                {
                    model.Deal = new LookupField { Key = dealInfo.Id, Value = dealInfo.Name };
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeOrderDeal(ChangeOrderDealViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }

            try
            {
                _publicService.Handle(new ChangeOrderDealRequest { DealId = model.Deal.Key, OrderId = model.OrderId });
                model.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, model, ex);
            }

            return View(model);
        }

        #region Change State

        [HttpGet, UseDependencyFields]
        public ActionResult ChangeState(long orderId, int oldState, int newState, long? inspectorId, long? sourceOrgUnitId)
        {
            if (!Enum.IsDefined(typeof(OrderState), oldState) || !Enum.IsDefined(typeof(OrderState), newState))
            {
                return null;
            }

            switch ((OrderState)newState)
            {
                // На утверждении
                case OrderState.OnApproval:
                    return View("ChangeStateOnApproval",
                                new ChangeOrderStateOnApprovalViewModel
                                {
                                    OrderId = orderId,
                                    Inspector = new LookupField { Key = inspectorId, Value = _userIdentifierService.GetUserInfo(inspectorId).DisplayName },
                                    SourceOrganizationUnitId = sourceOrgUnitId
                                });

                // На расторжении
                case OrderState.OnTermination:
                    var terminationInfo = _secureFinder.Find<Order>(x => x.Id == orderId).Select(x => new { x.TerminationReason, x.Comment }).Single();
                    return View("ChangeStateOnTermination",
                                new ChangeOrderStateOnTerminationViewModel
                                {
                                    OrderId = orderId,
                                    TerminationReason = (OrderTerminationReason)terminationInfo.TerminationReason,
                                    TerminationReasonComment = terminationInfo.Comment
                                });
                default:
                    return View("ChangeStateDefault", new ChangeOrderStateDefaultViewModel { OrderId = orderId });
            }
        }

        [HttpPost]
        public ActionResult ChangeStateOnApproval(ChangeOrderStateOnApprovalViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }

            _orderRepository.SetInspector(model.OrderId, model.Inspector.Key);

            // По значению OK в поле Message клиентский javascript понимает, что надо закрыть окно :-/
            model.Message = BLResources.OK;
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStateOnTermination(ChangeOrderStateOnTerminationViewModel model)
        {
            if (ModelUtils.CheckIsModelValid(this, model))
            {
                model.Message = BLResources.OK;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStateDefault(ChangeOrderStateDefaultViewModel model)
        {
            if (ModelUtils.CheckIsModelValid(this, model))
            {
                model.Message = BLResources.OK;
            }

            return View(model);
        }

        #endregion

        #region close with denial

        [HttpGet]
        [UseDependencyFields]
        public ActionResult CloseWithDenial(long? id)
        {
            if (!id.HasValue)
            {
                throw new NotificationException(BLResources.IdentifierNotSet);
            }

            var model = new CloseOrderWithDenialViewModel { OrderId = id.Value };

            var response = _orderReadModel.IsOrderDeactivationPossible(id.Value);

            if (response.IsDeactivationAllowed)
            {
                model.Confirmation = response.DeactivationConfirmation;
                model.CanClose = true;
            }
            else
            {
                model.SetCriticalError(response.DeactivationDisallowedReason);
            }

            return View("CloseWithDenial", model);
        }

        [HttpPost]
        public JsonNetResult CloseWithDenial(CloseOrderWithDenialViewModel model)
        {
            _publicService.Handle(new CloseOrderRequest { OrderId = model.OrderId, Reason = model.Reason });
            return new JsonNetResult();
        }

        [HttpGet]
        [UseDependencyFields]
        public ActionResult CrmCloseWithDenial(Guid[] crmIds)
        {
            if (crmIds.Length == 0)
            {
                throw new NotificationException("Выберите элемент");
            }

            if (crmIds.Length > 1)
            {
                throw new NotificationException("Выберите только один заказ");
            }

            return CloseWithDenial(_replicationCodeConverter.ConvertToEntityId(EntityName.Order, crmIds[0]));
        }

        #endregion

        #region printing
        [HttpGet]
        public ActionResult PrintOrder(long id, long profileId)
        {
            try
            {
                _publicService.Handle(new ChangeOrderLegalPersonProfileRequest { OrderId = id, LegalPersonProfileId = profileId });
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }

            return TryPrintDocument(new PrintOrderWithGuarateeRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintReferenceInformation(long id, long profileId)
        {
            return
                TryPrintDocument(new PrintReferenceInformationRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintRegionalOrder(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderRequest { OrderId = id, PrintRegionalVersion = true, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintBargain(long id, long? profileId)
        {
            return TryPrintDocument(new PrintOrderBargainRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintBill(long id, long? profileId)
        {
            return TryPrintDocument(new PrintOrderBillsRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintLetterOfGuarantee(long id, long? profileId)
        {
            return TryPrintDocument(new PrintLetterOfGuaranteeRequest { OrderId = id, LegalPersonProfileId = profileId, IsChangingAdvMaterial = true });
        }

        [HttpPost]
        public JsonNetResult GetRelatedOrdersInfoForPrintJointBill(long id)
        {
            var orderInfo = _secureFinder.Find<Order>(o => o.Id == id && o.IsActive && !o.IsDeleted).FirstOrDefault();
            if (orderInfo == null)
            {
                return new JsonNetResult(null);
            }

            var response = (GetRelatedOrdersForPrintJointBillResponse)_publicService.Handle(new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id });
            return new JsonNetResult(response.Orders);
        }

        [HttpGet]
        public ActionResult PrepareJointBill(long id, long? profileId)
        {
            var model = new PrepareJointBillViewModel
            {
                EntityId = id,
                EntityName = typeof(Order).AsEntityName(),
                ProfileId = profileId,
                IsMassBillCreateAvailable = false
            };

            var orderInfo = _secureFinder.Find<Order>(o => o.Id == id && o.IsActive && !o.IsDeleted).FirstOrDefault();
            if (orderInfo != null)
            {
                var response = (GetRelatedOrdersForPrintJointBillResponse)_publicService.Handle(new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id });
                model.IsMassBillCreateAvailable = response.Orders != null && response.Orders.Length > 0;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PrintJointBill(long id, string relatedOrders, long? profileId)
        {
            var orders = JsonConvert.DeserializeObject<long[]>(relatedOrders);

            if (orders != null && orders.Length > 0)
                return TryPrintDocument(new PrintOrderJointBillRequest { OrderId = id, RelatedOrderIds = orders, LegalPersonProfile = profileId });

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult PrintTerminationNotice(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintTerminationNoticeWithoutReason(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId, WithoutReason = true });
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNotice(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId, TerminationBargain = true });
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNoticeWithoutReason(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId, WithoutReason = true, TerminationBargain = true });
        }

        [HttpGet]
        public ActionResult PrintRegionalTerminationNotice(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintRegionalOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId });
        }

        [HttpGet]
        public ActionResult PrintAdditionalAgreement(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderAdditionalAgreementRequest
                {
                    OrderId = id,
                    LegalPersonProfileId = profileId,
                    PrintType = PrintAdditionalAgreementTarget.Order
                });
        }

        [HttpGet]
        public ActionResult PrintBargainAdditionalAgreement(long id, long? profileId)
        {
            return
                TryPrintDocument(new PrintOrderAdditionalAgreementRequest
                {
                    OrderId = id,
                    LegalPersonProfileId = profileId,
                    PrintType = PrintAdditionalAgreementTarget.Bargain
                });
        }

        private ActionResult TryPrintDocument(Request request)
        {
            try
            {
                var response = (StreamResponse)_publicService.Handle(request);
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }
        }

        public ActionResult Print(PrintOrderType printOrderType, long orderId)
        {
            var order = _orderReadModel.GetOrder(orderId);
            if (!order.LegalPersonId.HasValue)
            {
                throw new ArgumentException("LegalPersonId");
            }

            var printOrderModel = new PrintOrderViewModel
            {
                LegalPersonId = order.LegalPersonId.Value,
                OrderId = orderId,
                PrintOrderType = printOrderType
            };

            return View(printOrderModel);
        }

        public JsonNetResult IsChooseProfileNeeded(long orderId, PrintOrderType printOrderType)
        {
            var isChooseProfileNeeded = true;
            long? legalPersonProfile = null;

            var order = _orderReadModel.GetOrder(orderId);

            if (order.LegalPersonProfileId.HasValue && printOrderType != PrintOrderType.PrintOrder)
            {
                isChooseProfileNeeded = false;
                legalPersonProfile = order.LegalPersonProfileId.Value;
            }
            else if (order.LegalPersonId.HasValue)
            {
                var legalPersonWithProfiles =
                    _legalPersonRepository.GetLegalPersonWithProfiles(order.LegalPersonId.Value);
                if ((LegalPersonType)legalPersonWithProfiles.LegalPerson.LegalPersonTypeEnum ==
                    LegalPersonType.NaturalPerson
                    && printOrderType != PrintOrderType.PrintOrder
                    && printOrderType != PrintOrderType.PrintBargain
                    && printOrderType != PrintOrderType.PrintReferenceInformation)
                {
                    isChooseProfileNeeded = false;
                }

                if (legalPersonWithProfiles.Profiles.Count() == 1)
                {
                    isChooseProfileNeeded = false;
                    legalPersonProfile = legalPersonWithProfiles.Profiles.First().Id;
                }
            }

            return new JsonNetResult(new
            {
                IsChooseProfileNeeded = isChooseProfileNeeded,
                LegalPersonProfileId = legalPersonProfile
            });
        }
        #endregion

        // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 выполнен формальный рефакторинг Handler->OperationService, при рефакторинге необходимо было реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном методе отпадет
        [HttpPost]
        public JsonNetResult CopyOrder(long orderId, bool isTechnicalTermination)
        {
            var response = _copyOrderOperationService.CopyOrder(
                orderId,
                isTechnicalTermination);

            var upgradeResponse = _repairOutdatedPositionsOperationService.RepairOutdatedPositions(response.OrderId);

            return new JsonNetResult(new
            {
                Messages = upgradeResponse,
                OrderId = response.OrderId,
                OrderNumber = response.OrderNumber
            });
        }

        // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 - в данному случае необходимо реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном методе отпадет
        [HttpPost]
        public JsonNetResult ProlongateOrderForOrderProcessingRequest(long orderProcessingRequestId)
        {
            var response = _orderProlongationOperation.ProcessSingle(orderProcessingRequestId);

            return new JsonNetResult(new
            {
                Messages = response.Messages,
                OrderId = response.OrderId,
                OrderNumber = response.OrderNumber
            });
        }

        [HttpPost]
        public JsonNetResult CreateOrderForOrderProcessingRequest(long orderProcessingRequestId)
        {
            var response = _orderCreationOperation.ProcessSingle(orderProcessingRequestId);

            return new JsonNetResult(new
            {
                Messages = response.Messages,
                OrderId = response.Order.Id,
                OrderNumber = response.Order.Number,
            });
        }

        // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 выполнен формальный рефакторинг Handler->OperationService, при рефакторинге необходимо было реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном методе отпадет
        public ActionResult RepairOutdatedOrderPositions(long orderId)
        {
            var orderInfo = _secureFinder.Find<Order>(order => order.Id == orderId).FirstOrDefault();

            if (orderInfo == null)
            {
                throw new NotificationException(BLResources.OrderNotFound);
            }

            var response = _repairOutdatedPositionsOperationService.RepairOutdatedPositions(orderId);
            return new JsonNetResult(new { Messages = response });
        }
    }

    // TODO {all, 13.11.2013}: перенос старого cr - убрать этот класс нафиг
    public static class ConfigUtil
    {
        public static ToolbarJson FindCardToolbarItem(this EntityViewConfig config, string toolBarItemName, bool throwIfNotFound = true)
        {
            var result = config.CardSettings.CardToolbar.FirstOrDefault(x => string.Equals(x.Name, toolBarItemName, StringComparison.OrdinalIgnoreCase));
            if (result == null && throwIfNotFound)
            {
                throw new ArgumentException(string.Format("Cannot find toolbar item '{0}' in config for entity '{1}'", toolBarItemName, config.EntityName));
            }

            return result;
        }

        public static void DisableCardToolbarItem(this EntityViewConfig config, string toolBarItemName, bool throwIfNotFound = true)
        {
            var item = FindCardToolbarItem(config, toolBarItemName, throwIfNotFound);
            if (item != null)
            {
                item.Disabled = true;
            }
        }

        public static void EnableCardToolbarItem(this EntityViewConfig config, string toolBarItemName, bool throwIfNotFound = true)
        {
            var item = FindCardToolbarItem(config, toolBarItemName, throwIfNotFound);
            if (item != null)
            {
                item.Disabled = false;
            }
        }
    }
}