using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrderDtoService : GetDomainEntityDtoServiceBase<Order>
    {
        private const int DefaultReleaseCount = 4;

        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly ISecureFinder _finder;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IUserReadModel _userReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IClientReadModel _clientReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public GetOrderDtoService(IUserContext userContext,
                                  ISecureFinder finder,
                                  ISecurityServiceFunctionalAccess functionalAccessService,
                                  IOrderReadModel orderReadModel,
                                  IFirmReadModel firmReadModel,
                                  IBranchOfficeRepository branchOfficeRepository,
                                  IUserReadModel userReadModel,
                                  ILegalPersonReadModel legalPersonReadModel,
                                  IBusinessModelSettings businessModelSettings,
                                  IClientReadModel clientReadModel,
                                  IBranchOfficeReadModel branchOfficeReadModel)
            : base(userContext)
        {
            _finder = finder;
            _functionalAccessService = functionalAccessService;
            _orderReadModel = orderReadModel;
            _firmReadModel = firmReadModel;
            _branchOfficeRepository = branchOfficeRepository;
            _userReadModel = userReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _businessModelSettings = businessModelSettings;
            _clientReadModel = clientReadModel;            
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override IDomainEntityDto<Order> GetDto(long entityId)
        {
            var dto = _finder.Find(OrderSpecs.Orders.Select.OrderDomainEntityDto(), Specs.Find.ById<Order>(entityId)).Single();

            dto.ShowRegionalAttributes = dto.SourceOrganizationUnitRef.Id != dto.DestOrganizationUnitRef.Id &&
                !_orderReadModel.CheckIsBranchToBranchOrder(dto.SourceOrganizationUnitRef.Id.Value, dto.DestOrganizationUnitRef.Id.Value, false);

            // В представление отдаем значение скидки, округленный до 2-х знаков
            // То же делается на клиентской стороне при асинхронных пересчетах при изменении этих полей
            if (dto.DiscountSum.HasValue && dto.DiscountPercent.HasValue)
            {
                dto.DiscountSum = RoundValueToSignificantDigits(dto.DiscountSum.Value);
            }

            dto.PayablePlan = RoundValueToSignificantDigits(dto.PayablePlan);
            dto.PayableFact = RoundValueToSignificantDigits(dto.PayableFact);
            dto.PayablePrice = RoundValueToSignificantDigits(dto.PayablePrice);
            dto.VatPlan = RoundValueToSignificantDigits(dto.VatPlan);
            dto.AmountWithdrawn = RoundValueToSignificantDigits(dto.AmountWithdrawn);
            dto.AmountToWithdraw = RoundValueToSignificantDigits(dto.AmountToWithdraw);

            return dto;
        }

        protected override IDomainEntityDto<Order> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (parentEntityName != EntityName.Deal)
            {
                // Для создания заказа не из сделки нужен специальный пермишен!
                var hasExtendedCreationPrivilege =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderCreationExtended, UserContext.Identity.Code);

                if (!hasExtendedCreationPrivilege)
                {
                    throw new BusinessLogicException(BLResources.AccessDeniedCreateOrderFromList);
                }
            }
            else if (parentEntityName == EntityName.Deal && parentEntityId == null)
            {
                throw new BusinessLogicException(BLResources.DealNotSpecifiedDuringOrderCreation);
            }

            var resultDto = CreateDtoWithFixedFieldSet();

            FillDtoWithUserDefinedValues(resultDto, UserContext.Identity.Code);

            FillDtoWithParentEntityDefinedValues(resultDto, parentEntityName, parentEntityId);

            FillAutocalculatedFields(resultDto);

            if (resultDto.ClientRef != null && resultDto.ClientRef.Id.HasValue && _clientReadModel.IsClientInReserve(resultDto.ClientRef.Id.Value))
            {
                throw new BusinessLogicException(string.Format(BLResources.CanNotCreateOrderWithReservedClient, resultDto.ClientRef.Name));
            }

            if (resultDto.FirmRef != null && resultDto.FirmRef.Id.HasValue && _firmReadModel.IsFirmInReserve(resultDto.FirmRef.Id.Value))
            {
                throw new BusinessLogicException(string.Format(BLResources.CanNotCreateOrderWithReservedFirm, resultDto.FirmRef.Name));
            }

            return resultDto;
        }

        private OrderDomainEntityDto CreateDtoWithFixedFieldSet()
        {
            var utcNow = DateTime.UtcNow;
            var distributionDatesDto = _orderReadModel.CalculateDistributionDates(utcNow.GetNextMonthFirstDate(), DefaultReleaseCount, DefaultReleaseCount);

            return new OrderDomainEntityDto
            {
                IsActive = true,
                ReleaseCountPlan = DefaultReleaseCount,
                ReleaseCountFact = DefaultReleaseCount,
                HasDocumentsDebt = DocumentsDebt.Absent,
                CreatedOn = utcNow,
                SignupDate = utcNow,
                PreviousWorkflowStepId = OrderState.OnRegistration,
                WorkflowStepId = OrderState.OnRegistration,
                BeginDistributionDate = distributionDatesDto.BeginDistributionDate,
                EndDistributionDatePlan = distributionDatesDto.EndDistributionDatePlan,
                EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact,
            };
        }

        private void FillDtoWithUserDefinedValues(OrderDomainEntityDto resultDto, long userId)
        {
            // Город назначения определяется по пользователю, но только в том случае, если заказ создан не из сделки.
            // Кроме того, у пользователя город должен быть опредён однозначно - иначе не используем.
            var organizationUnitId = _userReadModel.GetUserOrganizationUnitId(userId);
            if (organizationUnitId.HasValue)
            {
                var refs = _orderReadModel.GetFieldValuesByOrganizationUnit(organizationUnitId.Value);

                resultDto.CurrencyRef = refs.Currency;
                resultDto.SourceOrganizationUnitRef = refs.OrganizationUnit;
            }
        }

        private void FillDtoWithParentEntityDefinedValues(OrderDomainEntityDto resultDto, EntityName parentEntityName, long? parentEntityId)
        {
            if (!parentEntityId.HasValue)
            {
                return;
            }

            if (parentEntityName == EntityName.Firm && _firmReadModel.HasFirmClient(parentEntityId.Value) == false)
            {
                throw new BusinessLogicException(BLResources.CannotCreateOrderForFirmWithoutClient);
            }

            var refs = _orderReadModel.GetOrderFieldValuesByParentEntity(parentEntityName, parentEntityId.Value);

            resultDto.ClientRef = refs.Client;
            resultDto.FirmRef = refs.Firm;
            resultDto.LegalPersonRef = refs.LegalPerson;
            resultDto.DealRef = refs.Deal;
            resultDto.OwnerRef = refs.Owner;
            resultDto.DealCurrencyId = refs.DealCurrency != null ? refs.DealCurrency.Id : null;

            // может так получиться, что по родительской сущности город не определён, зато был определён ранее по пользователю - в таком случае старое значение не перетираем пустым.
            resultDto.DestOrganizationUnitRef = refs.DestOrganizationUnit ?? resultDto.DestOrganizationUnitRef;
        }

        private void FillAutocalculatedFields(OrderDomainEntityDto resultDto)
        {
            if (resultDto.LegalPersonRef != null && resultDto.LegalPersonRef.Id.HasValue)
            {
                resultDto.PaymentMethod = _legalPersonReadModel.GetPaymentMethod(resultDto.LegalPersonRef.Id.Value) ?? PaymentMethod.Undefined;
            }

            if (resultDto.DestOrganizationUnitRef != null && resultDto.DestOrganizationUnitRef.Id.HasValue)
            {
                var releaseNumbersDto = _orderReadModel.CalculateReleaseNumbers(resultDto.DestOrganizationUnitRef.Id.Value,
                                                                                resultDto.BeginDistributionDate,
                                                                                DefaultReleaseCount,
                                                                                DefaultReleaseCount);
                resultDto.BeginReleaseNumber = releaseNumbersDto.BeginReleaseNumber;
                resultDto.EndReleaseNumberPlan = releaseNumbersDto.EndReleaseNumberPlan;
                resultDto.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;
            }

            if (resultDto.DestOrganizationUnitRef != null && resultDto.DestOrganizationUnitRef.Id.HasValue)
            {
                var priceWasPublished = _orderReadModel.OrderPriceWasPublished(resultDto.DestOrganizationUnitRef.Id.Value, resultDto.BeginDistributionDate);
                resultDto.HasDestOrganizationUnitPublishedPrice = priceWasPublished;
            }

            resultDto.BranchOfficeOrganizationUnitRef =
                DetermineBranchOfficeOrganizationUnit(resultDto.SourceOrganizationUnitRef != null ? resultDto.SourceOrganizationUnitRef.Id : null);
        }

        private EntityReference DetermineBranchOfficeOrganizationUnit(long? sourceOrganizationUnitId)
        {
            var userBranchOfficeIds = _userReadModel.GetUserBranchOffices(UserContext.Identity.Code);
            if (userBranchOfficeIds.Any())
            {
                var branchOfficeOrganizationUnits = _branchOfficeReadModel.GetBranchOfficeOrganizationUnitIdsAndNamesByBranchOfficeIds(userBranchOfficeIds);
                if (branchOfficeOrganizationUnits.Count() == 1)
                {
                    var branchOfficeOrganizationUnitInfo = branchOfficeOrganizationUnits.Single().Value;
                    return new EntityReference(branchOfficeOrganizationUnitInfo.Item1, branchOfficeOrganizationUnitInfo.Item2);
                }
            }
            else if (sourceOrganizationUnitId.HasValue)
            {
                var branchOfficeOrganizationUnitShortInfo = _branchOfficeRepository.GetBranchOfficeOrganizationUnitShortInfo(sourceOrganizationUnitId.Value);
                return new EntityReference(branchOfficeOrganizationUnitShortInfo.Id, branchOfficeOrganizationUnitShortInfo.ShortLegalName);
            }

            return null;
        }

        private decimal RoundValueToSignificantDigits(decimal value)
        {
            // COMMENT {all, 18.09.2014}: А в чем смысл? 
            // Если после значимых стоят нули - то округляй, не округляй, значение не изменится. А если не нули - то не округляем.
            // Т.е. получается, что значение не меняется никогда?

            // TODO {y.baranihin, 05.02.2014}: Перенести форматирование денег на клиент
            // если внезапно за значимыми знаками стоят не 0, то округлять не будем
            var x = value * (long)Math.Pow(10, _businessModelSettings.SignificantDigitsNumber);
            if ((x - (long)x) != 0)
            {
                return value;
            }

            return Math.Round(value, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
        }
    }
}