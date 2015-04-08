using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.SourceSamples
{
    public class OrderController : ReplicableEntityController<Order, OrderModel>
    {
        private readonly IOrderService _orderService;
        private readonly IAppSettings _appSettings;
        private readonly IReleaseInfoService _releaseInfoService;
        private readonly IOrganizationUnitService _organizationUnitService;
        private readonly ICrudService<UserTerritoriesOrganizationUnits> _userTerritoriesOrganizationUnitsService;
        private readonly ICrudService<ActionsHistory> _actionsHistoryService;
        private readonly IOperationService _operationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public OrderController(
            IAppSettings appSettings,
            IUserContext userContext,
            ICommonLog logger,
            ISecurityServiceUserIdentifier userIdentifierService,
            IQueryService<Order> queryService,
            [UnsecureErmScope]IQueryService<Order> unsecureQueryService,
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            ISecurityServiceSharings securityServiceSharings,
            IPublicService publicService,
            IReleaseInfoService releaseInfoService,
            IOrderService orderService,
            IOrganizationUnitService organizationUnitService,
            ICrudService<UserTerritoriesOrganizationUnits> userTerritoriesOrganizationUnitsService,
            ICrudService<ActionsHistory> actionsHistoryService,
            IOperationService operationService)
            : base(
                appSettings,
                userContext,
                logger,
                userIdentifierService,
                queryService,
                unsecureQueryService,
                configurationService,
                entityAccessService,
                functionalAccessService,
                securityServiceSharings,
                publicService)
        {
            _appSettings = appSettings;
            _releaseInfoService = releaseInfoService;
            _orderService = orderService;
            _organizationUnitService = organizationUnitService;
            _userTerritoriesOrganizationUnitsService = userTerritoriesOrganizationUnitsService;
            _actionsHistoryService = actionsHistoryService;
            _operationService = operationService;
            _functionalAccessService = functionalAccessService;
        }

        protected override void CustomizeModelAfterMetadataReady(OrderModel model)
        {
            if (model.IsNew)
            {
                return;
            }

            if (!model.IsActive)
            {
                model.LockToolbar();
                return;
            }

            if (model.WorkflowStepId == (int)OrderState.Approved || model.WorkflowStepId == (int)OrderState.OnTermination)
            {
                var isReleaseInProgress = _releaseInfoService.HasFinalRelease(model.DestinationOrganizationUnit.Key, new TimePeriod(model.BeginDistributionDate, model.EndDistributionDateFact), ReleaseInfoStatus.InProgress);
                if (isReleaseInProgress)
                {
                    model.LockToolbar();
                    model.SetWarning("Идет сборка. Редактирование заказа запрещено.");
                }
            }

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderChangeDealExtended, UserContext.Identity.Code))
            {
                model.ViewConfig.DisableCardToolbarItem("ChangeDeal");
            }

            // Кнопки изменения договора закрыты при наличии сборки
            var hasLocks = _orderService.QueryRead().Where(order => order.Id == model.Id).SelectMany(order => order.Locks).Any(@lock => !@lock.IsDeleted);
            if (hasLocks)
            {
                var bargainButtons = model.ViewConfig.CardSettings.CardToolbar
                    .Where(x => string.Equals(x.Name, "CreateBargain", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, "RemoveBargain", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                Array.ForEach(bargainButtons, item => item.Disabled = true);
            }

            {   // restrict printing of termination notice and additional agreement
                var isActionDisabledBasedOnWorkflowStepId = !model.IsTerminated || !(model.WorkflowStepId == (int)OrderState.OnTermination || model.WorkflowStepId == (int)OrderState.Archive);
                if (isActionDisabledBasedOnWorkflowStepId)
                {
                    model.ViewConfig.DisableCardToolbarItem("PrintTerminationNoticeAction");
                    model.ViewConfig.DisableCardToolbarItem("PrintAdditionalAgreementAction");
                }

                var isReqionalOrder = model.OriginalSteObject.SourceOrganizationUnitId !=
                                      model.OriginalSteObject.DestOrganizationUnitId;
            }

            var disableCheckOrder = !(model.WorkflowStepId == (int)OrderState.OnApproval || model.WorkflowStepId == (int)OrderState.Approved || model.WorkflowStepId == (int)OrderState.OnRegistration);
            if (disableCheckOrder)
            {
                model.ViewConfig.DisableCardToolbarItem("CheckOrder");
            }
            else
            {
                model.ViewConfig.EnableCardToolbarItem("CheckOrder");
            }
        }

        protected override OrderModel GetViewModel(int? id, bool? readOnly, int? pId, EntityName pType, string extendedInfo)
        {
            var currentUserCode = UserContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => FunctionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            OrderModel orderModel;
            if (id.HasValue)
            {
                orderModel = GetModelById(id.Value);
            }
            else
            {
                if (pType == EntityName.None ||
                    pType == EntityName.Client ||
                    pType == EntityName.LegalPerson ||
                    pType == EntityName.Account ||
                    pType == EntityName.Firm)
                {
                    /* Для создания заказа не из сделки нужен специальный пермишен! */
                    var hasExtendedCreationPrivilege = functionalPrivilegeValidator(FunctionalPrivilegeName.OrderCreationExtended);
                    if (!hasExtendedCreationPrivilege)
                    {
                        throw new NotificationException(Resources.AccessDeniedCreateOrderFromList);
                    }
                }
                else if (pType == EntityName.Deal && pId == null)
                {
                    throw new NotificationException(Resources.DealNotSpecifiedDuringOrderCreation);
                }

                var dealId = pType == EntityName.Deal ? pId : null;
                var response = (CreateOrderResponse)PublicService.Handle(new CreateOrderRequest { DealId = dealId });
                orderModel = response.OrderDto.ToViewModel(response);
            }

            ModelState.SetModelValue("WorkflowStepId", new ValueProviderResult(orderModel.PreviousWorkflowStepId, orderModel.PreviousWorkflowStepId.ToString(CultureInfo.InvariantCulture), null));

            orderModel.CurrenctUserCode = currentUserCode;
            orderModel.Inspector.Value = UserIdentifierService.GetUserInfo(orderModel.Inspector.Key).DisplayName;
            orderModel.AvailableSteps = GetAvailableSteps(orderModel.Id, orderModel.IsNew, (OrderState)orderModel.WorkflowStepId, orderModel.SourceOrganizationUnit.Key);

            // Проверить функциональные разрешения
            orderModel.HasOrderCreationExtended = functionalPrivilegeValidator(FunctionalPrivilegeName.OrderCreationExtended);
            orderModel.CanEditOrderType = functionalPrivilegeValidator(FunctionalPrivilegeName.EditOrderType);

            // Карточка только на чтение если не "на регистрациии"
            orderModel.ViewConfig.ReadOnly = orderModel.WorkflowStepId != (int)OrderState.OnRegistration;

            if (!orderModel.IsActive)
            {
                orderModel.MessageType = MessageType.Warning;
                orderModel.Message = Resources.WarningOrderIsRejected;
            }
            return orderModel;
        }

        private OrderModel GetModelById(int id)
        {
            var model = _orderService.QueryRead()
                    .Where(x => x.Id == id)
                    .Select(x => new OrderModel
            {
                OriginalSteObject = x,

                Id = x.Id,
                OrderNumber = x.Number,
                RegionalNumber = x.RegionalNumber,
                Firm = new LookupField { Key = x.FirmId, Value = x.Firm.Name },
                ClientId = (x.Deal != null) ? x.Deal.ClientId : x.Firm.ClientId,
                DgppId = x.DgppId,
                HasAnyOrderPosition = x.OrderPositions.Any(op => op.IsActive && !op.IsDeleted),
                HasDestOrganizationUnitPublishedPrice = x.DestOrganizationUnit.Prices.Any(price => price.IsPublished && price.IsActive && !price.IsDeleted && price.BeginDate <= x.BeginDistributionDate),
                SourceOrganizationUnit = new LookupField { Key = x.SourceOrganizationUnitId, Value = x.SourceOrganizationUnit.Name },
                DestinationOrganizationUnit = new LookupField { Key = x.DestOrganizationUnitId, Value = x.DestOrganizationUnit.Name },
                BranchOfficeOrganizationUnit = new LookupField { Key = x.BranchOfficeOrganizationUnitId, Value = x.BranchOfficeOrganizationUnit.ShortLegalName },
                LegalPerson = new LookupField { Key = x.LegalPersonId, Value = x.LegalPerson.LegalName },
                Deal = new LookupField { Key = x.DealId, Value = x.Deal.Name },
                DealCurrencyId = x.Deal.CurrencyId,
                Currency = new LookupField { Key = x.CurrencyId, Value = x.Currency.Name },
                BeginDistributionDate = x.BeginDistributionDate,
                EndDistributionDatePlan = x.EndDistributionDatePlan,
                EndDistributionDateFact = x.EndDistributionDateFact,
                BeginReleaseNumber = x.BeginReleaseNumber,
                EndReleaseNumberPlan = x.EndReleaseNumberPlan,
                EndReleaseNumberFact = x.EndReleaseNumberFact,
                SignupDate = x.SignupDate,
                ReleaseCountPlan = x.ReleaseCountPlan,
                ReleaseCountFact = x.ReleaseCountFact,
                PreviousWorkflowStepId = x.WorkflowStepId,
                WorkflowStepId = x.WorkflowStepId,
                PayablePlan = x.PayablePlan,
                PayableFact = x.PayableFact,
                PayablePrice = x.PayablePrice,
                VatPlan = x.VatPlan,
                AmountToWithdraw = x.AmountToWithdraw,
                AmountWithdrawn = x.AmountWithdrawn,
                DiscountSum = x.DiscountSum,
                DiscountPercent = x.DiscountPercent,
                DiscountReason = (OrderDiscountReason)x.DiscountReasonEnum,
                DiscountComment = x.DiscountComment,
                DiscountPercentChecked = x.OrderPositions.Where(y => !y.IsDeleted && y.IsActive).All(y => y.CalculateDiscountViaPercent),
                Comment = x.Comment,
                IsTerminated = x.IsTerminated,
                TerminationReason = (OrderTerminationReason)x.TerminationReason,
                OrderType = (OrderType)x.OrderType,
                Inspector = new LookupField { Key = x.InspectorCode, Value = null },
                Bargain = new LookupField { Key = x.BargainId, Value = x.Bargain.Number },
                BudgetType = (OrderBudgetType)x.BudgetType,
                Owner = new LookupField { Key = x.OwnerCode, Value = null },
                Platform = x.Platform == null ? string.Empty : x.Platform.Name,
                PlatformId = x.PlatformId,
                HasDocumentsDebt = (OrderHasDocumentsDebt)x.HasDocumentsDebt,
                DocumentsComment = x.DocumentsComment,
                AccountId = x.AccountId
            })
                    .Single();

            // Проверка на возможность отображения кнопки "Перейти к лицевому счету"
            var accountInfo = QueryService.QueryRead()
                .Where(x => x.Id == id && x.Account != null)
                .Select(x => new { x.Account.Id, x.Account.OwnerCode })
                .SingleOrDefault();
            if (accountInfo != null)
            {
                if (UserContext.Identity.SkipEntityAccessCheck)
                {
                    model.CanSwitchToAccount = true;
                }
                else
                {
                    model.CanSwitchToAccount = EntityAccessService.HasEntityAccess(EntityAccessTypes.Read,
                                                                                   EntityName.Account,
                                                                                   UserContext.Identity.Code,
                                                                                   accountInfo.Id,
                                                                                   accountInfo.OwnerCode,
                                                                                   null);
                }
            }

            // ShowRegionalAttributes
            if (model.SourceOrganizationUnit != null && model.DestinationOrganizationUnit != null &&
                model.SourceOrganizationUnit.Key != model.DestinationOrganizationUnit.Key)
            {
                var isBranchToBranch = _organizationUnitService.CheckIsBranchToBranchOrder(model.SourceOrganizationUnit.Key.Value,
                                                                                           model.DestinationOrganizationUnit.Key.Value,
                                                                                           false);
                model.ShowRegionalAttributes = !isBranchToBranch;
            }

            // В представление отдаем значение скидки и процент скидки, округленный до 2-х знаков
            // То же делается на клиентской стороне при асинхронных пересчетах при изменении этих полей
            if (model.DiscountSum.HasValue && model.DiscountPercent.HasValue)
            {
                model.DiscountSum = Math.Round(model.DiscountSum.Value, 2, MidpointRounding.ToEven);
                model.DiscountPercent = Math.Round(model.DiscountPercent.Value, 2, MidpointRounding.ToEven);
            }
            return model;
        }

        private string GetAvailableSteps(int orderId, bool isNew, OrderState currentState, int? sourceOrganizationUnitId)
        {
            var resultList = new List<OrderState> { currentState };

            if (!isNew)
            {
                var response = (AvailableTransitionsResponse)PublicService.Handle(new AvailableTransitionsRequest
                {
                    OrderId = orderId,
                    CurrentState = currentState,
                    SourceOrganizationUnitId = sourceOrganizationUnitId
                });
                resultList.AddRange(response.AvailableTransitions);
            }

            return JsonConvert.SerializeObject(resultList.ConvertAll(state => new
            {
                Value = state.ToString("D"),
                Text = state.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
            }));
        }

        protected override EditRequest<Order> CreateEditRequest(OrderModel model)
        {
            return new EditOrderRequest
            {
                DiscountInPercents = model.DiscountPercentChecked,
                ReservedNumberDigit = model.ReservedNumberDigit,
                NewWorkflowStepId = model.WorkflowStepId
            };
        }

        [HttpPost, DeserializeOriginalModel, UseDependencyFields]
        public override ActionResult Edit(OrderModel model)
        {
            model.EndDistributionDatePlan = model.EndDistributionDatePlan.GetEndPeriodOfThisMonth();
            model.EndDistributionDateFact = model.EndDistributionDateFact.GetEndPeriodOfThisMonth();

            var ste = model.OriginalSteObject;
            var isOneOfOrganizationUnitChanged = model.DestinationOrganizationUnit.Key != ste.DestOrganizationUnitId ||
                                                 model.SourceOrganizationUnit.Key != ste.SourceOrganizationUnitId;
            if (ste.ChangeTracker.State == ObjectState.Added || isOneOfOrganizationUnitChanged)
            {
                model.ReservedNumberDigit = _orderService.GenerateNextOrderUniqueNumber();
            }

            return base.Edit(model);
        }

        // Необходимо для журналирования изменений конкретной сущности
        [LogWebRequest(EntityName.Order, DeepCompare = true, ElementsToIgnore = "OrderPositions, OrderReleaseTotals, Account, *.Count")]
        protected override OrderModel EditInternal(OrderModel model)
        {
            return base.EditInternal(model);
        }

        #region Ajax methods

        [HttpPost]
        public JsonNetResult GetReleasesNumbers(int? organizationUnitid, DateTime? beginDistributionDate, int? releaseCountPlan)
        {
            if (!organizationUnitid.HasValue || !beginDistributionDate.HasValue || !releaseCountPlan.HasValue)
            {
                return new JsonNetResult();
            }

            var numbers = (CalculateOrderReleasesResponse)PublicService.Handle(new CalculateOrderReleasesRequest
            {
                CalculatedBeginDistributionDate = beginDistributionDate.Value,
                OrganizationUnitId = organizationUnitid.Value,
                ReleaseCountPlan = releaseCountPlan.Value
            });

            return new JsonNetResult(new
            {
                numbers.BeginReleaseNumber,
                numbers.EndReleaseNumberPlan,
                numbers.EndReleaseNumberFact,
                BeginDistributionDate = numbers.BeginDistributionDate.ToString(CultureInfo.InvariantCulture),
                EndDistributionDate = numbers.EndDistributionDate.ToString(CultureInfo.InvariantCulture)
            });
        }

        [HttpPost]
        public JsonNetResult GetCurrency(int? organizationUnitid)
        {
            if (!organizationUnitid.HasValue)
            {
                return new JsonNetResult();
            }

            var currencyInfo = _organizationUnitService.QueryRead()
                .Where(x => !x.IsDeleted && x.IsActive && x.Id == organizationUnitid.Value)
                .Select(x => new { x.Country.Currency.Id, x.Country.Currency.Name }).FirstOrDefault();

            return (currencyInfo == null)
                       ? new JsonNetResult()
                       : new JsonNetResult(new { currencyInfo.Id, currencyInfo.Name });
        }

        [HttpPost]
        public JsonNetResult GetBranchOfficeOrganizationUnit(int? organizationUnitid)
        {
            if (!organizationUnitid.HasValue)
            {
                return new JsonNetResult();
            }

            var response = (GetOrderBranchOfficeOrganizationUnitResponse)PublicService.Handle(new GetOrderBranchOfficeOrganizationUnitRequest { OrganizationUnitId = organizationUnitid.Value });
            return new JsonNetResult(new { Id = response.BranchOfficeOrganizationUnitId, Name = response.BranchOfficeOrganizationUnitName });
        }

        [HttpPost]
        public JsonNetResult GetLegalPerson(int? firmClientId)
        {
            if (!firmClientId.HasValue)
            {
                return new JsonNetResult();
            }

            var resp = (GetOrderLegalPersonResponse)PublicService.Handle(new GetOrderLegalPersonRequest { FirmClientId = firmClientId.Value });
            return new JsonNetResult(new { Id = resp.LegalPersonId, Name = resp.LegalPersonName });
        }

        public JsonNetResult GetHasDestOrganizationUnitPublishedPrice(int? orderId, int? orgUnitId)
        {
            if (!orderId.HasValue || !orgUnitId.HasValue)
            {
                return new JsonNetResult();
            }

            var beginDistributionDate = _orderService.QueryRead()
                    .Where(o => o.Id == orderId.Value)
                    .Select(o => o.BeginDistributionDate)
                    .FirstOrDefault();

            var hasDestOrganizationUnitPublishedPrice = _organizationUnitService.QueryRead()
                    .Where(ou => ou.Id == orgUnitId.Value)
                    .Any(ou => ou.Prices.Any(price => price.IsPublished && price.IsActive && !price.IsDeleted && price.BeginDate <= beginDistributionDate));

            return new JsonNetResult(hasDestOrganizationUnitPublishedPrice);
        }

        [HttpPost]
        public JsonNetResult GetDestinationOrganizationUnit(int? firmId)
        {
            if (!firmId.HasValue)
            {
                return new JsonNetResult();
            }

            var resp = (GetOrderDestinationOrganizationUnitResponse)PublicService.Handle(new GetOrderDestinationOrganizationUnitRequest { FirmId = firmId.Value });
            return new JsonNetResult(new { Id = resp.OrganizationUnitId, Name = resp.OrganizationUnitName });
        }

        [HttpPost]
        public JsonNetResult DiscountRecalc(RecalculateOrderDiscountRequest request)
        {
            try
            {
                var response = (RecalculateOrderDiscountResponse)PublicService.Handle(request);
                return new JsonNetResult(response);
            }
            catch (Exception ex)
            {
                var model = new ViewModel();
                OnException(model, ex);
                return new JsonNetResult(new { model.Message, model.MessageType });
            }
        }

        [HttpPost]
        public JsonNetResult ChangeOrderType(int orderId, string orderType)
        {
            if (orderId == 0)
            {
                return new JsonNetResult();
            }

            try
            {
                if (!Enum.IsDefined(typeof(OrderType), orderType))
                {
                    throw new NotificationException("Неверный тип заказа");
                }

                var order = _orderService.QueryModify().Single(x => x.Id == orderId);
                order.OrderType = (int)Enum.Parse(typeof(OrderType), orderType);
                _orderService.Modify(order);

                return new JsonNetResult();
            }
            catch (Exception ex)
            {
                var tmpModel = new ViewModel();
                OnException(tmpModel, ex);
                return new JsonNetResult(new { tmpModel.Message, tmpModel.MessageType });
            }
        }

        [HttpGet]
        public JsonNetResult GetOrderAggregateInCurrentState(long id)
        {
            var orderAggregate = _orderService.QueryRead()
                .Where(x => x.Id == id)
                .Select(x => new
            {
                Order = x,

                Platform = x.Platform.Name,
                DiscountReason = (OrderDiscountReason)x.DiscountReasonEnum,
                BudgetType = (OrderBudgetType)x.BudgetType,
                DiscountInPercents = x.OrderPositions
                                 .Where(y => !y.IsDeleted && y.IsActive)
                                 .All(y => y.CalculateDiscountViaPercent),
            })
                .Single();
            return new JsonNetResult(orderAggregate);
        }

        [HttpGet]
        public JsonNetResult CanCreateOrderPositionsForOrder(long orderId)
        {
            var canCreateResponse = (CanCreateOrderPositionForOrderResponse)
                                    PublicService.Handle(new CanCreateOrderPositionForOrderRequest { OrderId = orderId });
            return new JsonNetResult(canCreateResponse);
        }

        [HttpPost]
        public JsonNetResult CheckBeginDistributionDate(int orderId, DateTime beginDistributionDate, int sourceOrganizationUnitId, int destinationOrganizationUnitId)
        {
            PublicService.Handle(new CheckOrderBeginDistributionDateRequest
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

            var firstUserOrgUnit = _userTerritoriesOrganizationUnitsService.QueryRead()
                .Where(x => x.OwnerCode == currentUser.Code)
                .Select(x => new
            {
                x.OrganizationUnitId,
                OrganizationUnitName = x.OrganizationUnit.Name
            })
                .FirstOrDefault();
            var model = new CheckOrdersReadinessForReleaseDialogModel
            {
                StartPeriodDate = nextMonth.GetFirstDateOfMonth(),
                Owner = new LookupField
                {
                    Key = currentUser.Code,
                    Value = UserIdentifierService.GetUserInfo(currentUser.Code).DisplayName
                },
                OrganizationUnit = new LookupField
                {
                    Key = firstUserOrgUnit != null ? firstUserOrgUnit.OrganizationUnitId : (int?)null,
                    Value = firstUserOrgUnit != null ? firstUserOrgUnit.OrganizationUnitName : null
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckOrdersReadinessForReleaseDialog(CheckOrdersReadinessForReleaseDialogModel model)
        {
            try
            {
                if (!FunctionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.PrereleaseOrderValidationExecution, UserContext.Identity.Code))
                {
                    throw new NotificationException(Resources.AccessDeniedPrereleaseOrderValidationExecution);
                }

                var isReleaseInProgress = _releaseInfoService.QueryRead()
                    .Any(x => !x.IsBeta &&
                              x.OrganizationUnitId == model.OrganizationUnit.Key &&
                              x.Status == (int)ReleaseInfoStatus.InProgress);
                if (isReleaseInProgress)
                {
                    throw new NotificationException(string.Format(Resources.ReleaseIsInProgressForOrganizationUnit, model.OrganizationUnit.Value));
                }

                var endPeriodDate = model.StartPeriodDate.GetEndPeriodOfThisMonth();
                var response = (CheckOrdersReadinessForReleaseResponse)PublicService.Handle(new CheckOrdersReadinessForReleaseRequest
                {
                    OrganizationUnitId = model.OrganizationUnit.Key,
                    OwnerId = model.OrganizationUnit != null ? model.Owner.Key : null,
                    IncludeOwnerDescendants = model.IncludeOwnerDescendants,
                    CheckAccountBalance = model.CheckAccountBalance,
                    Period = new TimePeriod(model.StartPeriodDate, endPeriodDate)
                });

                model.Message = "Проверка завершена";
                if (response.HasErrors)
                {
                    model.Message = response.Message;
                    model.HasErrors = true;
                    Guid operationId = Guid.NewGuid();
                    _operationService.FinishOperation(operationId, response.ReportContent, HttpUtility.UrlPathEncode(response.ReportFileName), response.ContentType);
                    model.ErrorLogFileId = operationId;
                }
                else
                {
                    model.Message = response.Message;
                    model.HasErrors = false;
                }
            }
            catch (Exception ex)
            {
                OnException(model, ex);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult RemoveBargain(int orderId)
        {
            PublicService.Handle(new RemoveBargainFromOrderRequest { OrderId = orderId });
            return null;
        }

        [HttpPost]
        public JsonNetResult GetBargainRemovalConfirmation(int orderId)
        {
            var hasAnotherOrders = _orderService.QueryRead().Where(order => order.Id == orderId)
                                                                        .Select(order => order.Bargain.Orders.Any(item => item.Id != orderId && !item.IsDeleted))
                                                                        .Single();
            return new JsonNetResult(hasAnotherOrders
                       ? "Будет удалена связь договора с данным заказом; сам договор удалён не будет. Продолжить?"
                       : "Договор будет удалён безвозвратно. Продолжить?");
        }

        [HttpGet]
        public ActionResult ChangeOrderDeal(long orderId)
        {
            var model = new ChangeOrderDealModel();
            if (orderId != 0)
            {
                var dealInfo = QueryService.QueryRead()
                    .Where(x => x.Id == orderId)
                    .Select(x => new { Id = (int?)x.Deal.Id, x.Deal.Name })
                    .SingleOrDefault();
                if (dealInfo != null)
                {
                    model.Deal = new LookupField { Key = dealInfo.Id, Value = dealInfo.Name };
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeOrderDeal(ChangeOrderDealModel model)
        {
            if (!CheckIsModelValid(model))
            {
                return View(model);
            }

            try
            {
                PublicService.Handle(new ChangeOrderDealRequest { DealId = model.Deal.Key, OrderId = model.OrderId });
                model.Message = Resources.OK;
            }
            catch (Exception ex)
            {
                OnException(model, ex);
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
                case OrderState.OnApproval: //На утверждении
                    return View("ChangeStateOnApproval", new ChangeOrderStateOnApprovalModel
                    {
                        OrderId = orderId,
                        Inspector = new LookupField { Key = inspectorId, Value = UserIdentifierService.GetUserInfo(inspectorId).DisplayName },
                        SourceOrganizationUnitId = sourceOrgUnitId
                    });
                case OrderState.OnTermination: //На расторжении
                    var terminationInfo = QueryService.QueryRead().Where(x => x.Id == orderId).Select(x => new { x.TerminationReason, x.Comment }).Single();
                    return View("ChangeStateOnTermination", new ChangeOrderStateOnTerminationModel
                    {
                        OrderId = orderId,
                        TerminationReason = (OrderTerminationReason)terminationInfo.TerminationReason,
                        TerminationReasonComment = terminationInfo.Comment
                    });
                default:
                    return View("ChangeStateDefault", new ChangeOrderStateDefaultModel { OrderId = orderId });
            }
        }

        [HttpPost]
        public ActionResult ChangeStateOnApproval(ChangeOrderStateOnApprovalModel model)
        {
            if (!CheckIsModelValid(model))
            {
                return View(model);
            }

            var order = _orderService.QueryModify().Single(x => x.Id == model.OrderId);

            order.InspectorCode = model.Inspector.Key;
            _orderService.Modify(order);

            model.Message = Resources.OK;
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStateOnTermination(ChangeOrderStateOnTerminationModel model)
        {
            if (CheckIsModelValid(model))
            {
                model.Message = Resources.OK;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStateDefault(ChangeOrderStateDefaultModel model)
        {
            if (CheckIsModelValid(model))
            {
                model.Message = Resources.OK;
            }

            return View(model);
        }

        private  Request GetRequest()
        {
            new VerifyOrderStateRequest { OrderId = orderId, NewOrderState = newOrderState };
        }

        [HttpPost]
        public JsonNetResult VerifyOrderState(int orderId, int newOrderState)
        {
            try
            {
                var response = (ValidateOrdersResponse)PublicService.Handle(this.GetRequest(this.GetRequest(this.GetRequest())));
                return new JsonNetResult(response);
            }
            catch (Exception ex)
            {
                var tmpModel = new ViewModel();
                OnException(tmpModel, ex);
                return new JsonNetResult(new { IsError = true, tmpModel.Message, tmpModel.MessageType });
            }
        }

        #endregion


        #region close with denial

        [HttpGet]
        [UseDependencyFields]
        public ActionResult CloseWithDenial(long? id)
        {
            if (!id.HasValue)
            {
                throw new NotificationException(Resources.IdentifierNotSet);
            }

            var model = new CloseOrderWithDenialModel { OrderId = id.Value };

            var response = _orderRepository.IsOrderDeactivationPossible(id.Value);

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
        public JsonNetResult CloseWithDenial(CloseOrderWithDenialModel model)
        {
            PublicService.Handle(new CloseOrderRequest { OrderId = model.OrderId, Reason = model.Reason });
            return new JsonNetResult();
        }

        [HttpGet]
        [UseDependencyFields]
        public ActionResult CrmCloseWithDenial(Guid? id)
        {
            return CloseWithDenial(GetIdByReplicationCode(id));
        }

        #endregion

        #region printing

        [HttpGet]
        public ActionResult PrintOrder(long id)
        {
            return TryPrintDocument(new PrintOrderRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintRegionalOrder(long id)
        {
            return TryPrintDocument(new PrintOrderRequest { OrderId = id, PrintRegionalVersion = true });
        }

        [HttpGet]
        public ActionResult PrintBargain(long id)
        {
            return TryPrintDocument(new PrintOrderBargainRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintBill(long id)
        {
            return TryPrintDocument(new PrintOrderBillsRequest { OrderId = id });
        }

        [HttpPost]
        public JsonNetResult GetRelatedOrdersInfoForPrintJointBill(long id)
        {
            var orderInfo = QueryService.QueryRead().FirstOrDefault(o => o.Id == id && o.IsActive && !o.IsDeleted);
            if (orderInfo == null)
            {
                return new JsonNetResult(null);
            }

            var response = (GetRelatedOrdersForPrintJointBillResponse)PublicService.Handle(new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id });
            return new JsonNetResult(response.Orders);
        }

        [HttpGet]
        public ActionResult PrepareJointBill(long id)
        {
            var model = new PrepareJointBillModel
            {
                EntityId = id,
                EntityName = typeof(Order).AsEntityName(),
                IsMassBillCreateAvailable = false
            };

            var orderInfo = QueryService.QueryRead().FirstOrDefault(o => o.Id == id && o.IsActive && !o.IsDeleted);
            if (orderInfo != null)
            {
                var response = (GetRelatedOrdersForPrintJointBillResponse)PublicService.Handle(new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id });
                model.IsMassBillCreateAvailable = response.Orders != null && response.Orders.Length > 0;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PrintJointBill(int id, string relatedOrders)
        {
            var orders = JsonConvert.DeserializeObject<int[]>(relatedOrders);

            if (orders != null && orders.Length > 0)
                return TryPrintDocument(new PrintOrderJointBillRequest { OrderId = id, RelatedOrderIds = orders });

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult PrintTerminationNotice(long id)
        {
            return TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintAdditionalAgreement(long id)
        {
            return TryPrintDocument(new PrintOrderAdditionalAgreementRequest { OrderId = id });
        }

        private ActionResult TryPrintDocument(Request request)
        {
            try
            {
                var response = (StreamResponse)PublicService.Handle(request);
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }
        }

        #endregion

        [HttpPost]
        public JsonNetResult CopyOrder(int orderId, bool isTechnicalTermination)
        {
            var orderNumber = _orderService.GenerateNextOrderUniqueNumber();
            var response = (CopyOrderResponse)PublicService.Handle(new CopyOrderRequest
            {
                OrderId = orderId,
                IsTechnicalTermination = isTechnicalTermination,
                OrderNumber = orderNumber
            });
            var upgradeResponse = (RepairOutdatedOrderPositionsResponse)PublicService.Handle(new RepairOutdatedOrderPositionsRequest { OrderId = response.OrderId });
            foreach (var message in upgradeResponse.Messages)
                response.Messages.Add(message);
            return new JsonNetResult(response);
        }

        [HttpGet]
        public override JsonNetResult GetActionsHistory(long entityId)
        {
            var ahddata = _actionsHistoryService.QueryRead()
                .Where(ah => ah.EntityType == (int)EntityName.Order && ah.EntityId == entityId)
                .SelectMany(ah => ah.ActionsHistoryDetails)
                .Where(ahd => ahd.PropertyName == "WorkflowStepId")
                .Select(ahd => new
            {
                ahd.Id,
                ahd.PropertyName,
                ahd.OriginalValue,
                ahd.ModifiedValue,
                ahd.ActionsHistoryId,
            }).ToArray();

            var ahdtdata = ahddata.Select(ahd => new
            {
                ahd.Id,
                ahd.ActionsHistoryId,
                PropertyName = Resources.OrderState,
                OriginalValue = ((OrderState)int.Parse(ahd.OriginalValue)).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                ModifiedValue = ((OrderState)int.Parse(ahd.ModifiedValue)).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
            });

            var ahdata = _actionsHistoryService.QueryRead()
                            .Where(ah => ah.EntityType == (int)EntityName.Order && ah.EntityId == entityId && ah.ActionsHistoryDetails.Any(ahd => ahd.PropertyName == "WorkflowStepId"))
                            .Select(ahd => new
            {
                ahd.Id,
                ActionType = (ActionType)ahd.ActionType,
                ahd.CreatedBy,
                ahd.CreatedOn,
            }).ToArray();

            var ahtData = ahdata.Select(x => new
            {
                x.Id,
                ActionType = x.ActionType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                CreatedBy = UserIdentifierService.GetUserInfo(x.CreatedBy).DisplayName,
                x.CreatedOn
            });
            return new JsonNetResult(new { ActionHistoryDetailsData = ahdtdata, ActionHistoryData = ahtData });
        }

        public ActionResult RepairOutdatedOrderPositions(int orderId)
        {
            Debug.WriteLine(orderId);

            var orderInfo = _orderService.QueryRead()
                .FirstOrDefault(order => order.Id == orderId);

            if (orderInfo == null)
                throw new NotificationException("Заказ не найден");

            var response = PublicService.Handle(new RepairOutdatedOrderPositionsRequest { OrderId = orderId });
            return new JsonNetResult(response);
        }
    }

    // TODO: убрать этот класс нафиг
    public static class ConfigUtil
    {
        public static ToolbarJson FindCardToolbarItem(this EntityViewConfig config, string toolBarItemName)
        {
            var result = config.CardSettings.CardToolbar.FirstOrDefault(x => string.Equals(x.Name, toolBarItemName, StringComparison.OrdinalIgnoreCase));
            if (result == null)
            {
                throw new ArgumentException(string.Format("Cannot find toolbar item '{0}' in config for entity '{1}'", toolBarItemName, config.EntityName));
            }

            return result;
        }

        public static void DisableCardToolbarItem(this EntityViewConfig config, string toolBarItemName)
        {
            var item = FindCardToolbarItem(config, toolBarItemName);
            item.Disabled = true;
        }

        public static void EnableCardToolbarItem(this EntityViewConfig config, string toolBarItemName)
        {
            var item = FindCardToolbarItem(config, toolBarItemName);
            item.Disabled = false;
        }
    }
}
