using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Resources.Server;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly ICopyOrderOperationService _copyOrderOperationService;
        private readonly IFinder _finder;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationService _operationService;

        private readonly IProcessOrderCreationRequestSingleOperation _orderCreationOperation;
        private readonly IProcessOrderProlongationRequestSingleOperation _orderProlongationOperation;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderRepository _orderRepository;
        private readonly IPublicService _publicService;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IRepairOutdatedPositionsOperationService _repairOutdatedPositionsOperationService;
        private readonly IReplicationCodeConverter _replicationCodeConverter;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IDetermineOrderBargainOperationService _determineOrderBargainOperationService;
        private readonly IChangeOrderLegalPersonProfileOperationService _changeOrderLegalPersonProfileOperationService;
        private readonly ICheckIfOrderPositionCanBeCreatedForOrderOperationService _checkIfOrderPositionCanBeCreatedForOrderOperationService;
        private readonly IGetOrderDocumentsDebtOperationService _getOrderDocumentsDebtOperationService;
        private readonly ISetOrderDocumentsDebtOperationService _setOrderDocumentsDebtOperationService;

        public OrderController(IMsCrmSettings msCrmSettings,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IIdentityServiceClientSettings identityServiceSettings,
                               IUserContext userContext,
                               ITracer tracer,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               ICopyOrderOperationService copyOrderOperationService,
                               IFinder finder,
                               ISecurityServiceFunctionalAccess functionalAccessService,
                               IOperationService operationService,
                               IProcessOrderCreationRequestSingleOperation orderCreationOperation,
                               IProcessOrderProlongationRequestSingleOperation orderProlongationOperation,
                               IOrderReadModel orderReadModel,
                               IOrderRepository orderRepository,
                               IPublicService publicService,
                               IReleaseReadModel releaseReadModel,
                               IRepairOutdatedPositionsOperationService repairOutdatedPositionsOperationService,
                               IReplicationCodeConverter replicationCodeConverter,
                               ISecureFinder secureFinder,
                               ISecurityServiceUserIdentifier userIdentifierService,
                               IDetermineOrderBargainOperationService determineOrderBargainOperationService,
                               ICheckIfOrderPositionCanBeCreatedForOrderOperationService checkIfOrderPositionCanBeCreatedForOrderOperationService,
                               IChangeOrderLegalPersonProfileOperationService changeOrderLegalPersonProfileOperationService,
                               IGetOrderDocumentsDebtOperationService getOrderDocumentsDebtOperationService,
                               ISetOrderDocumentsDebtOperationService setOrderDocumentsDebtOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _copyOrderOperationService = copyOrderOperationService;
            _finder = finder;
            _functionalAccessService = functionalAccessService;
            _operationService = operationService;
            _orderCreationOperation = orderCreationOperation;
            _orderProlongationOperation = orderProlongationOperation;
            _orderReadModel = orderReadModel;
            _orderRepository = orderRepository;
            _publicService = publicService;
            _releaseReadModel = releaseReadModel;
            _repairOutdatedPositionsOperationService = repairOutdatedPositionsOperationService;
            _replicationCodeConverter = replicationCodeConverter;
            _secureFinder = secureFinder;
            _userIdentifierService = userIdentifierService;
            _determineOrderBargainOperationService = determineOrderBargainOperationService;
            _checkIfOrderPositionCanBeCreatedForOrderOperationService = checkIfOrderPositionCanBeCreatedForOrderOperationService;
            _changeOrderLegalPersonProfileOperationService = changeOrderLegalPersonProfileOperationService;
            _getOrderDocumentsDebtOperationService = getOrderDocumentsDebtOperationService;
            _setOrderDocumentsDebtOperationService = setOrderDocumentsDebtOperationService;
        }

        #region Ajax methods

        [HttpPost]
        public JsonNetResult GetReleasesNumbers(long? organizationUnitid, DateTime? beginDistributionDate, int? releaseCountPlan)
        {
            if (!organizationUnitid.HasValue || !beginDistributionDate.HasValue || !releaseCountPlan.HasValue)
            {
                return new JsonNetResult();
            }

            var releaseNumbersDto = _orderReadModel.CalculateReleaseNumbers(organizationUnitid.Value,
                                                                            beginDistributionDate.Value,
                                                                            releaseCountPlan.Value,
                                                                            releaseCountPlan.Value);
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

            var currencyInfo = _secureFinder.Find(new FindSpecification<OrganizationUnit>(x => !x.IsDeleted && x.IsActive && x.Id == organizationUnitid.Value))
                                            .Select(x => new { x.Country.Currency.Id, x.Country.Currency.Name }).FirstOrDefault();

            return (currencyInfo == null)
                       ? new JsonNetResult()
                       : new JsonNetResult(new { currencyInfo.Id, currencyInfo.Name });
        }

        public JsonNetResult GetHasDestOrganizationUnitPublishedPrice(long? orderId, long? orgUnitId)
        {
            if (!orderId.HasValue || !orgUnitId.HasValue)
            {
                return new JsonNetResult();
            }

            var beginDistributionDate = _secureFinder.Find(new FindSpecification<Order>(o => o.Id == orderId.Value)).Select(o => o.BeginDistributionDate).FirstOrDefault();

            var hasDestOrganizationUnitPublishedPrice =
                _secureFinder.Find(new FindSpecification<OrganizationUnit>(ou => ou.Id == orgUnitId.Value))
                             .Any(ou =>
                                  ou.Prices.Any(price => price.IsPublished && price.IsActive && !price.IsDeleted && price.BeginDate <= beginDistributionDate));

            return new JsonNetResult(hasDestOrganizationUnitPublishedPrice);
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
                ModelUtils.OnException(this, Tracer, model, ex);
                return new JsonNetResult(new { model.Message, model.MessageType });
            }
        }

        // TODO {d.ivanov, y.baranihin, 25.02.2013}: Учесть возможность явного получения текущего состояния агрегата, без использования скрытых полей типа SerializeSteObject
        [HttpGet]
        public JsonNetResult GetOrderAggregateInCurrentState(long id)
        {
            var orderAggregate = _secureFinder.Find(new FindSpecification<Order>(x => x.Id == id))
                                              .Select(x => new
                                                  {
                                                      Order = x,
                                                      Platform = x.Platform.Name,
                                                      DiscountReason = x.DiscountReasonEnum,
                                                      DiscountInPercents = x.OrderPositions
                                                                            .Where(y => !y.IsDeleted && y.IsActive)
                                                                            .All(y => y.CalculateDiscountViaPercent),
                                                      x.Timestamp
                                                  })
                                              .AsEnumerable()
                                              .Select(x => new
                                                  {
                                                      // ReSharper disable RedundantAnonymousTypePropertyName
                                                      Order = x.Order,
                                                      Platform = x.Platform,
                                                      DiscountReason = x.DiscountReason,
                                                      DiscountInPercents = x.DiscountInPercents,
                                                      // ReSharper restore RedundantAnonymousTypePropertyName
                                                      EntityStateToken = Convert.ToBase64String(x.Timestamp)
                                                  })
                                              .Single();

            return new JsonNetResult(orderAggregate);
        }

        [HttpGet]
        public JsonNetResult CanCreateOrderPositionsForOrder(long orderId, string orderTypeValue)
        {
            OrderType orderType;
            string report;
            if (!Enum.TryParse(orderTypeValue, out orderType))
            {
                return new JsonNetResult(new
            {
                                                 CanCreate = false,
                                                 Message = BLResources.WrongOrderType
                    });
            }

            return new JsonNetResult(new
                                         {
                                             CanCreate = _checkIfOrderPositionCanBeCreatedForOrderOperationService.Check(orderId, orderType, out report),
                                             Message = report
                                         });
        }

        [HttpPost]
        public JsonNetResult CheckBeginDistributionDate(long orderId,
                                                        DateTime beginDistributionDate,
                                                        long sourceOrganizationUnitId,
                                                        long destinationOrganizationUnitId)
        {
            _publicService.Handle(new CheckOrderBeginDistributionDateRequest
                {
                    OrderId = orderId,
                    BeginDistributionDate = beginDistributionDate.Date, // FIXME {all, 29.10.2014}: Костыль на тему часовых посов. Если приходит 2014-01-01T01:00, то это вовсе не значит, что заказ хотят разместить начиная с часу ночи, просто в браузере ФИЗИЧЕСКИ нет возможности выбрать 2014-01-01T00:00
                    SourceOrganizationUnitId = sourceOrganizationUnitId,
                    DestinationOrganizationUnitId = destinationOrganizationUnitId
                });
            return new JsonNetResult();
        }

        [HttpPost]
        public JsonNetResult TryDetermineBargain(long? branchOfficeOrganizationUnitId,
                                                 long? legalPersonId,
                                                 DateTime endDistributionDate)
        {
            if (!branchOfficeOrganizationUnitId.HasValue || !legalPersonId.HasValue)
            {
                return new JsonNetResult();
            }

            long bargainId;
            string bargainNumber;

            if (!_determineOrderBargainOperationService.TryDetermineOrderBargain(legalPersonId.Value,
                                                                                 branchOfficeOrganizationUnitId.Value,
                                                                                 endDistributionDate,
                                                                                 out bargainId,
                                                                                 out bargainNumber))
            {
                return new JsonNetResult();
            }

            return new JsonNetResult(new { Id = bargainId, BargainNumber = bargainNumber });
        }

        #endregion

        [HttpGet]
        public ViewResult SelectLegalPersonProfile(long orderId)
        {
            var dto = _orderReadModel.GetLegalPersonProfileByOrder(orderId);

            var model = new SelectLegalPersonProfileViewModel
            {
                LegalPerson = dto.LegalPerson.ToLookupField(),
                LegalPersonProfile = dto.LegalPersonProfile.ToLookupField(),
            };

            return View(model);
        }

        [HttpPost]
        public EmptyResult ChangeOrderLegalPersonProfile(long orderId, long legalPersonProfileId)
        {
            _changeOrderLegalPersonProfileOperationService.ChangeLegalPersonProfile(orderId, legalPersonProfileId);
            return new EmptyResult();
        }

        // TODO {all, 18.03.2015}: Есть понимание, что это должно быть вынесено.
        [HttpGet]
        public ViewResult SetOrderDocumentsDebt(long orderId)
        {
            var dto = _getOrderDocumentsDebtOperationService.Get(orderId);

            var model = new SetOrderDocumentsDebtViewModel
            {
                Order = dto.Order.ToLookupField(),
                DocumentsComment = dto.DocumentsComment,
                HasDocumentsDebt = dto.HasDocumentsDebt
            };

            return View(model);
        }

        // TODO {all, 18.03.2015}: Есть понимание, что это должно быть вынесено.
        [HttpPost]
        public ViewResult SetOrderDocumentsDebt(SetOrderDocumentsDebtViewModel model)
        {
            _setOrderDocumentsDebtOperationService.Set(model.Order.Key.Value, model.HasDocumentsDebt, model.DocumentsComment);
            model.Message = BLResources.OK;
            return View(model);
        }        

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
                    throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.OrganizationUnit));
                }

                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PrereleaseOrderValidationExecution,
                                                                            UserContext.Identity.Code))
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
                    var operationDescription = string.Format(BLResources.CheckingOrdersHasFinishedWithErrors,
                                                             viewModel.OrganizationUnit.Value,
                                                             viewModel.StartPeriodDate.ToString("MMMM yyy"));

                    var operation = new Operation
                        {
                            Guid = operationId,
                            StartTime = DateTime.UtcNow,
                            FinishTime = DateTime.UtcNow,
                            OwnerCode = UserContext.Identity.Code,
                            Status = OperationStatus.Error,
                            Type = BusinessOperation.CheckOrdersReadinessForRelease,
                            Description = operationDescription,
                            OrganizationUnitId = viewModel.OrganizationUnit.Key
                        };

                    _operationService.CreateOperation(operation,
                                                      response.ReportContent,
                                                      HttpUtility.UrlPathEncode(response.ReportFileName),
                                                      response.ContentType);
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
                ModelUtils.OnException(this, Tracer, viewModel, ex);
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
                    throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.OrganizationUnit));
                }

                if (viewModel.Owner == null || !viewModel.Owner.Key.HasValue)
                {
                    throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Owner));
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
                            Status = OperationStatus.Success,
                            Type = BusinessOperation.GetOrdersWithDummyAdvertisements,
                            Description = operationDescription,
                            OrganizationUnitId = viewModel.OrganizationUnit.Key
                        };

                    _operationService.CreateOperation(operation,
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
                ModelUtils.OnException(this, Tracer, viewModel, ex);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ChangeOrderDeal(long orderId)
        {
            var model = new ChangeOrderDealViewModel();
            if (orderId != 0)
            {
                var dealInfo = _secureFinder.Find(new FindSpecification<Order>(x => x.Id == orderId))
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
                ModelUtils.OnException(this, Tracer, model, ex);
            }

            return View(model);
        }

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
                    response.OrderId,
                    response.OrderNumber
                });
        }

        // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 - в данному случае необходимо реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном методе отпадет
        [HttpPost]
        public JsonNetResult ProlongateOrderForOrderProcessingRequest(long orderProcessingRequestId)
        {
            var response = _orderProlongationOperation.ProcessSingle(orderProcessingRequestId);

            return new JsonNetResult(new
                {
                    response.Messages,
                    response.OrderId,
                    response.OrderNumber
                });
        }

        [HttpPost]
        public JsonNetResult CreateOrderForOrderProcessingRequest(long orderProcessingRequestId)
        {
            var response = _orderCreationOperation.ProcessSingle(orderProcessingRequestId);

            return new JsonNetResult(new
                {
                    response.Messages,
                    OrderId = response.Order.Id,
                    OrderNumber = response.Order.Number,
                });
        }

        // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 выполнен формальный рефакторинг Handler->OperationService, при рефакторинге необходимо было реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном методе отпадет
        public ActionResult RepairOutdatedOrderPositions(long orderId)
        {
            var orderInfo = _secureFinder.Find(new FindSpecification<Order>(order => order.Id == orderId)).FirstOrDefault();

            if (orderInfo == null)
            {
                throw new NotificationException(BLResources.OrderNotFound);
            }

            var response = _repairOutdatedPositionsOperationService.RepairOutdatedPositions(orderId);
            return new JsonNetResult(new { Messages = response });
        }

        #region Change State

        [HttpGet]
        [UseDependencyFields]
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
                    var terminationInfo = _secureFinder.Find(new FindSpecification<Order>(x => x.Id == orderId)).Select(x => new { x.TerminationReason, x.Comment }).Single();
                    return View("ChangeStateOnTermination",
                                new ChangeOrderStateOnTerminationViewModel
                                    {
                                        OrderId = orderId,
                                        TerminationReason = terminationInfo.TerminationReason,
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

            return CloseWithDenial(_replicationCodeConverter.ConvertToEntityId(EntityType.Instance.Order(), crmIds[0]));
        }

        #endregion
    }

    // TODO {all, 13.11.2013}: перенос старого cr - убрать этот класс нафиг
    public static class ConfigUtil
    {
        public static ToolbarElementStructure FindCardToolbarItem(this EntityViewConfig config, string toolBarItemName, bool throwIfNotFound = true)
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