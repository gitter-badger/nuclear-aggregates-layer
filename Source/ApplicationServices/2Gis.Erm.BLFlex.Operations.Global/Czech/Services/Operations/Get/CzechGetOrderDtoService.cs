using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.BranchOffices;
using DoubleGis.Erm.BL.Aggregates.Deals;
using DoubleGis.Erm.BL.Aggregates.Firms;
using DoubleGis.Erm.BL.Aggregates.Orders;
using DoubleGis.Erm.BL.Aggregates.Users;
using DoubleGis.Erm.BL.Operations.Operations.Generic.Get;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Services.Operations.Get
{
    public class CzechGetOrderDtoService : GetDomainEntityDtoServiceBase<Order>, ICzechAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IOrderRepository _orderRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IFirmRepository _firmRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;

        public CzechGetOrderDtoService(IUserContext userContext,
                                       ISecureFinder finder,
                                       ISecurityServiceFunctionalAccess functionalAccessService,
                                       ISecurityServiceEntityAccess entityAccessService,
                                       IOrderRepository orderRepository,
                                       IDealRepository dealRepository,
                                       IBranchOfficeRepository branchOfficeRepository,
                                       IFirmRepository firmRepository,
                                       IUserRepository userRepository)
            : base(userContext)
        {
            _finder = finder;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _entityAccessService = entityAccessService;
            _orderRepository = orderRepository;
            _dealRepository = dealRepository;
            _branchOfficeRepository = branchOfficeRepository;
            _firmRepository = firmRepository;
            _userRepository = userRepository;
        }

        protected override IDomainEntityDto<Order> GetDto(long entityId)
        {
            var dto = _finder.Find<Order>(x => x.Id == entityId)
                .Select(x => new OrderDomainEntityDto
                    {
                        Id = x.Id,
                        OrderNumber = x.Number,
                        RegionalNumber = x.RegionalNumber,
                        FirmRef = new EntityReference { Id = x.FirmId, Name = x.Firm.Name },
                        ClientRef = new EntityReference
                            {
                                Id = x.Deal != null ? x.Deal.ClientId : x.Firm.ClientId,
                                Name = (x.Deal != null) ? x.Deal.Client.Name : (x.Firm.Client != null ? x.Firm.Client.Name : null)
                            },
                        DgppId = x.DgppId,
                        HasAnyOrderPosition = x.OrderPositions.Any(op => op.IsActive && !op.IsDeleted),
                        HasDestOrganizationUnitPublishedPrice = x.DestOrganizationUnit.Prices
                                 .Any(price => price.IsPublished && price.IsActive &&
                                               !price.IsDeleted && price.BeginDate <= x.BeginDistributionDate),
                        SourceOrganizationUnitRef = new EntityReference { Id = x.SourceOrganizationUnitId, Name = x.SourceOrganizationUnit.Name },
                        DestOrganizationUnitRef = new EntityReference { Id = x.DestOrganizationUnitId, Name = x.DestOrganizationUnit.Name },
                        BranchOfficeOrganizationUnitRef = new EntityReference { Id = x.BranchOfficeOrganizationUnitId, Name = x.BranchOfficeOrganizationUnit.ShortLegalName },
                        LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = x.LegalPerson.LegalName },
                        DealRef = new EntityReference { Id = x.DealId, Name = x.Deal.Name },
                        DealCurrencyId = x.Deal.CurrencyId,
                        CurrencyRef = new EntityReference { Id = x.CurrencyId, Name = x.Currency.Name },
                        BeginDistributionDate = x.BeginDistributionDate,
                        EndDistributionDatePlan = x.EndDistributionDatePlan,
                        EndDistributionDateFact = x.EndDistributionDateFact,
                        BeginReleaseNumber = x.BeginReleaseNumber,
                        EndReleaseNumberPlan = x.EndReleaseNumberPlan,
                        EndReleaseNumberFact = x.EndReleaseNumberFact,
                        SignupDate = x.SignupDate,
                        ReleaseCountPlan = x.ReleaseCountPlan,
                        ReleaseCountFact = x.ReleaseCountFact,
                        PreviousWorkflowStepId = (OrderState)x.WorkflowStepId,
                        WorkflowStepId = (OrderState)x.WorkflowStepId,
                        PayablePlan = x.PayablePlan,
                        PayableFact = x.PayableFact,
                        PayablePrice = x.PayablePrice,
                        VatPlan = x.VatPlan,
                        AmountToWithdraw = x.AmountToWithdraw,
                        AmountWithdrawn = x.AmountWithdrawn,
                        DiscountSum = x.DiscountSum,
                        DiscountPercent = x.DiscountPercent,
                        DiscountReasonEnum = (OrderDiscountReason)x.DiscountReasonEnum,
                        DiscountComment = x.DiscountComment,
                        DiscountPercentChecked = x.OrderPositions
                                 .Where(y => !y.IsDeleted && y.IsActive)
                                 .All(y => y.CalculateDiscountViaPercent),
                        Comment = x.Comment,
                        IsTerminated = x.IsTerminated,
                        TerminationReason = (OrderTerminationReason)x.TerminationReason,
                        OrderType = (OrderType)x.OrderType,
                        InspectorRef = new EntityReference { Id = x.InspectorCode, Name = null },
                        BargainRef = new EntityReference { Id = x.BargainId, Name = x.Bargain.Number },
                        BudgetType = (OrderBudgetType)x.BudgetType,
                        Platform = x.Platform == null ? string.Empty : x.Platform.Name,
                        PlatformRef = new EntityReference { Id = x.PlatformId, Name = x.Platform == null ? string.Empty : x.Platform.Name },
                        HasDocumentsDebt = (DocumentsDebt)x.HasDocumentsDebt,
                        DocumentsComment = x.DocumentsComment,
                        AccountRef = new EntityReference { Id = x.AccountId, Name = null },

                        OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                        PaymentMethod = (PaymentMethod)x.PaymentMethod,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                        ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                        CreatedOn = x.CreatedOn,
                        ModifiedOn = x.ModifiedOn,
                        Timestamp = x.Timestamp
                    })
                .Single();

            // ѕроверка на возможность отображени€ кнопки "ѕерейти к лицевому счету"
            var accountInfo = _finder.Find<Order>(x => x.Id == entityId && x.Account != null)
                .Select(x => new { x.Account.Id, x.Account.OwnerCode })
                .SingleOrDefault();
            if (accountInfo != null)
            {
                if (_userContext.Identity.SkipEntityAccessCheck)
                {
                    dto.CanSwitchToAccount = true;
                }
                else
                {
                    dto.CanSwitchToAccount = _entityAccessService.HasEntityAccess(EntityAccessTypes.Read,
                                                                                  EntityName.Account,
                                                                                  _userContext.Identity.Code,
                                                                                  accountInfo.Id,
                                                                                  accountInfo.OwnerCode,
                                                                                  null);
                }
            }

            // ShowRegionalAttributes
            if (dto.SourceOrganizationUnitRef.Id != dto.DestOrganizationUnitRef.Id)
            {
                var isBranchToBranch = _orderRepository.CheckIsBranchToBranchOrder(dto.SourceOrganizationUnitRef.Id.Value,
                                                                                   dto.DestOrganizationUnitRef.Id.Value,
                                                                                   false);
                dto.ShowRegionalAttributes = !isBranchToBranch;
            }

            // ¬ представление отдаем значение скидки и процент скидки, округленный до 2-х знаков
            // “о же делаетс€ на клиентской стороне при асинхронных пересчетах при изменении этих полей
            if (dto.DiscountSum.HasValue && dto.DiscountPercent.HasValue)
            {
                dto.DiscountSum = Math.Round(dto.DiscountSum.Value, 2, MidpointRounding.ToEven);
                dto.DiscountPercent = Math.Round(dto.DiscountPercent.Value, 2, MidpointRounding.ToEven);
            }

            return dto;
        }

        protected override IDomainEntityDto<Order> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var currentUserCode = _userContext.Identity.Code;
            Func<FunctionalPrivilegeName, bool> functionalPrivilegeValidator =
                privilegeName => _functionalAccessService.HasFunctionalPrivilegeGranted(privilegeName, currentUserCode);

            if (parentEntityName == EntityName.None ||
                parentEntityName == EntityName.Client ||
                parentEntityName == EntityName.LegalPerson ||
                parentEntityName == EntityName.Account ||
                parentEntityName == EntityName.Firm)
            {
                // ƒл€ создани€ заказа не из сделки нужен специальный пермишен!
                var hasExtendedCreationPrivilege = functionalPrivilegeValidator(FunctionalPrivilegeName.OrderCreationExtended);
                if (!hasExtendedCreationPrivilege)
                {
                    throw new NotificationException(BLResources.AccessDeniedCreateOrderFromList);
                }
            }
            else if (parentEntityName == EntityName.Deal && parentEntityId == null)
            {
                throw new NotificationException(BLResources.DealNotSpecifiedDuringOrderCreation);
            }

            var dealId = parentEntityName == EntityName.Deal ? parentEntityId : null;
            var resultDto = CreateOrderDto(dealId);

            // ѕри создании заказа из клиента, фирмы и юр. лица заполн€ем соответсвующие пол€
            if (parentEntityId.HasValue
                && (parentEntityName == EntityName.Client || parentEntityName == EntityName.Firm || parentEntityName == EntityName.LegalPerson))
            {
                EntityReference clientRef;
                EntityReference firmRef;
                EntityReference legalPersonRef;
                if (TryGetReferences(parentEntityName, parentEntityId.Value, out clientRef, out firmRef, out legalPersonRef))
                {
                    resultDto.ClientRef = clientRef;
                    resultDto.FirmRef = firmRef;
                    resultDto.LegalPersonRef = legalPersonRef;
                }

                if (resultDto.LegalPersonRef != null && resultDto.LegalPersonRef.Id != 0)
                {
                    var paymentMethod = _finder.Find<LegalPersonProfile>(profile => profile.LegalPersonId == resultDto.LegalPersonRef.Id &&
                                                                                    profile.IsActive && !profile.IsDeleted && profile.IsMainProfile)
                        .Select(profile => profile.PaymentMethod).FirstOrDefault();
                    if (paymentMethod.HasValue)
                    {
                        resultDto.PaymentMethod = (PaymentMethod)paymentMethod;
                    }
                }
            }

            return resultDto;
        }

        private bool TryGetReferences(EntityName entityName,
                                      long entityId,
                                      out EntityReference clientRef,
                                      out EntityReference firmRef,
                                      out EntityReference legalPersonRef)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return GeLegalPersonReferenceByClient(entityId, out clientRef, out firmRef, out legalPersonRef);
                case EntityName.Firm:
                    return GeLegalPersonReferenceByFirm(entityId, out clientRef, out firmRef, out legalPersonRef);
                case EntityName.LegalPerson:
                    return GeLegalPersonReferenceByLegalPerson(entityId, out clientRef, out firmRef, out legalPersonRef);
                default:
                    return EmptyResult(out clientRef, out firmRef, out legalPersonRef);
            }
        }

        private bool GeLegalPersonReferenceByLegalPerson(long legalPersonId,
                                                         out EntityReference clientRef,
                                                         out EntityReference firmRef,
                                                         out EntityReference legalPersonRef)
        {
            var data = _finder.Find(GenericSpecifications.ById<LegalPerson>(legalPersonId))
                .Select(person => new
                    {
                        Client = new { person.Client.Id, person.Client.Name },
                        Firms = person.Client.Firms.Select(firm => new { firm.Id, firm.Name }),
                        LegalPerson = new { person.Id, person.ShortName },
                    })
                .SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef);
            }

            clientRef = data.Client != null ? new EntityReference(data.Client.Id, data.Client.Name) : null;
            firmRef = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
            legalPersonRef = new EntityReference(data.LegalPerson.Id, data.LegalPerson.ShortName);
            return true;
        }

        private bool GeLegalPersonReferenceByFirm(long firmId,
                                                  out EntityReference clientRef,
                                                  out EntityReference firmRef,
                                                  out EntityReference legalPersonRef)
        {
            var data = _finder.Find(GenericSpecifications.ById<Firm>(firmId))
                .Select(firm => new
                    {
                        Firm = new { firm.Id, firm.Name },
                        Client = firm.Client != null ? new EntityReference { Id = firm.Client.Id, Name = firm.Client.Name } : new EntityReference { Id = -1, Name = string.Empty },
                        LegalPersons = firm.Client.LegalPersons.Select(person => new { person.Id, person.ShortName })
                    }).SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef);
            }

            if (data.Client.Id == -1)
            {
                throw new NotificationException(BLResources.CannotCreateOrderForFirmWithoutClient);
            }

            clientRef = data.Client != null ? new EntityReference(data.Client.Id, data.Client.Name) : null;
            firmRef = new EntityReference(data.Firm.Id, data.Firm.Name);
            legalPersonRef = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().ShortName) : null;
            return true;
        }

        private bool GeLegalPersonReferenceByClient(long clientId,
                                                    out EntityReference clientRef,
                                                    out EntityReference firmRef,
                                                    out EntityReference legalPersonRef)
        {
            var data = _finder.Find(GenericSpecifications.ById<Client>(clientId))
                .Select(client => new
                    {
                        Client = new { client.Id, client.Name },
                        Firms = client.Firms.Select(firm => new { firm.Id, firm.Name }),
                        LegalPersons = client.LegalPersons.Select(person => new { person.Id, person.ShortName })
                    })
                .SingleOrDefault();

            if (data == null)
            {
                return EmptyResult(out clientRef, out firmRef, out legalPersonRef);
            }

            clientRef = new EntityReference(data.Client.Id, data.Client.Name);
            firmRef = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
            legalPersonRef = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().ShortName) : null;
            return true;
        }

        private bool EmptyResult(out EntityReference clientRef,
                                 out EntityReference firmRef,
                                 out EntityReference legalPersonRef)
        {
            clientRef = firmRef = legalPersonRef = null;
            return false;
        }

        private OrderDomainEntityDto CreateOrderDto(long? dealId)
        {
            const int ReleaseCount = 4;

            var utcNow = DateTime.UtcNow;
            var currentUserCode = _userContext.Identity.Code;

            var orderDto = new OrderDomainEntityDto
                {
                    IsActive = true,
                    ReleaseCountPlan = ReleaseCount,
                    ReleaseCountFact = ReleaseCount,
                    HasDocumentsDebt = DocumentsDebt.Absent,
                    CreatedOn = utcNow,
                    SignupDate = utcNow,
                    PreviousWorkflowStepId = OrderState.OnRegistration,
                    WorkflowStepId = OrderState.OnRegistration
                };

            var distributionDatesDto = _orderRepository.CalculateDistributionDates(utcNow.GetNextMonthFirstDate(), ReleaseCount, ReleaseCount);
            orderDto.BeginDistributionDate = distributionDatesDto.BeginDistributionDate;
            orderDto.EndDistributionDatePlan = distributionDatesDto.EndDistributionDatePlan;
            orderDto.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;

            // если создали заказ не из сделки, то пытаемс€ определить город назначени€ как город текущего пользовател€
            var organizationUnitDto = _userRepository.GetSingleOrDefaultOrganizationUnit(currentUserCode);
            if (organizationUnitDto != null)
            {
                // у пользовател€ должно быть одно отделение организации дл€ подстановки, а если больше - то пусть сам ручками заполн€ет.
                SetOrgUnitInfoByCurrentUser(orderDto, organizationUnitDto);

                var releaseNumbersDto = _orderRepository.CalculateReleaseNumbers(organizationUnitDto.Id, utcNow.GetNextMonthFirstDate(), ReleaseCount, ReleaseCount);
                orderDto.BeginReleaseNumber = releaseNumbersDto.BeginReleaseNumber;
                orderDto.EndReleaseNumberPlan = releaseNumbersDto.EndReleaseNumberPlan;
                orderDto.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;

                orderDto.HasDestOrganizationUnitPublishedPrice = _orderRepository.OrderPriceWasPublished(organizationUnitDto.Id, orderDto.BeginDistributionDate);
            }

            if (dealId.HasValue)
            {
                SetDealInfo(dealId.Value, orderDto);
            }

            return orderDto;
        }

        private void SetOrgUnitInfoByCurrentUser(OrderDomainEntityDto orderDto, OrganizationUnitDto organizationUnitDto)
        {
            orderDto.CurrencyRef = new EntityReference
            {
                Id = organizationUnitDto.CurrencyId,
                Name = organizationUnitDto.CurrencyName,
            };

            if (organizationUnitDto.ProjectExists)
            {
                orderDto.SourceOrganizationUnitRef = new EntityReference { Id = organizationUnitDto.Id, Name = organizationUnitDto.Name };
                orderDto.DestOrganizationUnitRef = new EntityReference { Id = organizationUnitDto.Id, Name = organizationUnitDto.Name };
            }

            // ѕолучить юр лицо исполнител€
            var branchOfficeOrganizationUnitShortInfo = _branchOfficeRepository.GetBranchOfficeOrganizationUnitShortInfo(organizationUnitDto.Id);
            orderDto.BranchOfficeOrganizationUnitRef = new EntityReference { Id = branchOfficeOrganizationUnitShortInfo.Id, Name = branchOfficeOrganizationUnitShortInfo.ShortLegalName };
        }

        private void SetDealInfo(long dealId, OrderDomainEntityDto orderDto)
        {
            var dealInfo = _dealRepository.GetDealLegalPerson(dealId);

            if (dealInfo == null)
            {
                throw new ArgumentException(string.Format(BLResources.DealForOrderNotFound, dealId));
            }

            if (dealInfo.LegalPerson != null)
            {
                orderDto.LegalPersonRef = new EntityReference { Id = dealInfo.LegalPerson.Id, Name = dealInfo.LegalPerson.Name };
            }

            // если дл€ сделки не была указана фирма, то пропустить установку значений из фирмы
            if (dealInfo.MainFirmId.HasValue)
            {
                var firmInfo = _firmRepository.GetFirmInBrief(dealInfo.MainFirmId.Value);

                if (firmInfo == null)
                {
                    throw new NullReferenceException(string.Format(BLResources.MainFirmForDealNotFound, dealInfo.MainFirmId));
                }

                // [MSCRM-2207] ѕри создании заказа из сделки в поле "√ород назначени€ " необходимо указывать отделение организации к которому прив€зана фирма (‘ирма -> ќтделение организации).
                orderDto.DestOrganizationUnitRef = new EntityReference { Id = firmInfo.OrganizationUnitId, Name = firmInfo.OrganizationUnitName };
                orderDto.FirmRef = new EntityReference { Id = firmInfo.FirmId, Name = firmInfo.FirmName };
            }

            if (dealInfo.OwnerCode > 0)
            {
                orderDto.OwnerRef = new EntityReference { Id = dealInfo.OwnerCode };
            }

            //  лиент - клиент сделки
            orderDto.ClientRef = new EntityReference { Id = dealInfo.ClientId };
            orderDto.DealRef = new EntityReference { Id = dealInfo.Id, Name = dealInfo.Name };

            orderDto.DealCurrencyId = dealInfo.CurrencyId;
        }
    }
}