using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BL.API.Operations.Metadata;
using DoubleGis.Erm.BL.Operations.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders.DTO;
using DoubleGis.Erm.Core.Services;
using DoubleGis.Erm.Core.Services.Operations.Concrete.List.DTO;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Services.Operations.List
{
    public class ListOrderService : ListEntityDtoServiceBase<Order, ListOrderDto>, IRussiaAdapted
    {
        private static readonly Func<OrderGridViewDto, ISecurityServiceUserIdentifier, IUserContext, ListOrderDto> ListDataSelectFunc =
            (order, userIdentifierService, userContext) =>
            new ListOrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.Number,
                    FirmId = order.FirmId,
                    FirmName = order.FirmName,
                    DestOrganizationUnitId = order.DestOrganizationUnitId,
                    DestOrganizationUnitName = order.DestOrganizationUnitName,
                    SourceOrganizationUnitId = order.SourceOrganizationUnitId,
                    SourceOrganizationUnitName = order.SourceOrganizationUnitName,
                    BeginDistributionDate = order.BeginDistributionDate,
                    EndDistributionDatePlan = order.EndDistributionDatePlan,
                    LegalPersonId = order.LegalPersonId,
                    LegalPersonName = order.LegalPersonName,
                    BargainId = order.BargainId,
                    BargainNumber = order.BargainNumber,
                    OwnerCode = order.OwnerCode,
                    OwnerName = userIdentifierService.GetUserInfo(order.OwnerCode).DisplayName,
                    WorkflowStep = ((OrderState)order.WorkflowStepId).ToStringLocalized(EnumResources.ResourceManager, userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    PayablePlan = order.PayablePlan,
                    AmountWithdrawn = order.AmountWithdrawn,
                    ModifiedOn = order.ModifiedOn
                };

        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;

        public ListOrderService(IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
                                ISecurityServiceUserIdentifier userIdentifierService,
                                IFinder finder,
                                IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
        }

        protected override IEnumerable<ListOrderDto> GetListData(IQueryable<Order> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var selectExpression = OrderSpecifications.Select.OrdersForGridView().Selector;

            var dummyAdvertisementsFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "WithDummyValues",
                withDummyValues =>
                {
                    if (!withDummyValues)
                    {
                        return null;
                    }

                    var dummyAdvertisementIds =
                        query.SelectMany(
                            x =>
                            x.OrderPositions.Where(y => !x.IsDeleted && x.IsActive)
                             .SelectMany(y => y.OrderPositionAdvertisements.Select(z => z.Advertisement.AdvertisementTemplate.DummyAdvertisementId)))
                             .Where(x => x != null)
                             .Distinct()
                             .ToArray();

                    return
                        x =>
                        x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                         .Any(y => y.OrderPositionAdvertisements.Any(z => dummyAdvertisementIds.Contains(z.AdvertisementId)));
                });

            var useCurrentMonthForEndDistributionDateFactFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "useCurrentMonthForEndDistributionDateFact",
                useCurrentMonth =>
                    {
                        if (!useCurrentMonth)
                        {
                            return null;
                        }

                        var nextMonth = DateTime.Now.AddMonths(1);
                        nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                        var currentMonth = nextMonth.AddSeconds(-1);

                        return x => x.EndDistributionDateFact == currentMonth;
                    });

            var forNextEditionFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "ForNextEdition",
                forNextEdition =>
                    {
                        if (!forNextEdition)
                        {
                            return null;
                        }

                        var nextMonth = DateTime.Now.AddMonths(1);
                        nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                        var currentMonthLastDate = nextMonth.AddSeconds(-1);
                        var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                        return
                            x => x.EndDistributionDateFact >= currentMonthLastDate && x.BeginDistributionDate <= currentMonthFirstDate;
                    });

            var forNextMonthEditionFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "ForNextMonthEdition",
                forNextMontEdition =>
                    {
                        if (!forNextMontEdition)
                        {
                            return null;
                        }

                        var tmpMonth = DateTime.Now.AddMonths(2);
                        tmpMonth = new DateTime(tmpMonth.Year, tmpMonth.Month, 1);

                        var nextMonthLastDate = tmpMonth.AddSeconds(-1);
                        var nextMonthFirstDate = new DateTime(nextMonthLastDate.Year, nextMonthLastDate.Month, 1);

                        return x => x.EndDistributionDateFact >= nextMonthLastDate && x.BeginDistributionDate <= nextMonthFirstDate;
                    });

            var withoutAdvertisementFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "WithoutAdvertisement",
                withoutAdvertisement =>
                    {
                        if (!withoutAdvertisement)
                        {
                            return null;
                        }

                        // ДгппИд элемента номенклатуры "пакет "Дополнительный"" нужен для костыля-исключения на 2+2 месяца (до Июля)
                        const int additionalPackageDgppId = 11572;

                        return x => x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                     .Select(z => new
                                         {
                                             RequiredPositionFails =
                                                      new[] { z.PricePosition.Position }
                                                      .Union(z.PricePosition.Position.ChildPositions
                                                              .Where(w => w.IsActive && !w.IsDeleted)
                                                              .Select(w => w.ChildPosition))
                                                      .Where(w => w.AdvertisementTemplate.IsAdvertisementRequired)
                                                      .Select(w => new
                                                          {
                                                              OpaIsEmpty = z.OrderPositionAdvertisements.All(p => p.PositionId != w.Id)
                                                                           &&
                                                                           z.PricePosition.Position.ChildPositions.All(
                                                                               m => m.MasterPosition.DgppId != additionalPackageDgppId),

                                                              AdvertisementIsRequired = z.OrderPositionAdvertisements.Where(p => p.PositionId == z.Id)
                                                                                         .Any(p => p.AdvertisementId == null),
                                                          })
                                         })
                                     .Any(y => y.RequiredPositionFails.Any(z => z.OpaIsEmpty || z.AdvertisementIsRequired));
                    });

            var rejectedByMeFilter = filterManager.CreateForExtendedProperty<Order, bool>(
                "RejectedByMe",
                rejectedByMe =>
                    {
                        if (!rejectedByMe)
                        {
                            return null;
                        }

                        var onApprovalState = ((int)OrderState.OnApproval).ToString();
                        var rejectedState = ((int)OrderState.Rejected).ToString();

                        var loqQuery = _finder.Find<ActionsHistoryDetail>(
                            x =>
                            x.ActionsHistory.EntityType == (int)EntityName.Order && x.PropertyName == "WorkflowStepId" &&
                            x.OriginalValue == onApprovalState && x.ModifiedValue == rejectedState)
                               .Select(x => x.ActionsHistory.EntityId);

                        return x => loqQuery.Contains(x.Id);
                    });

            switch (filterManager.ParentEntityName)
            {
                case EntityName.Client:
                    return query
                        .Where(x => x.Firm.ClientId == filterManager.ParentEntityId || x.LegalPerson.ClientId == filterManager.ParentEntityId)
                        .ApplyFilter(forNextEditionFilter)
                        .ApplyFilter(forNextMonthEditionFilter)
                        .ApplyFilter(withoutAdvertisementFilter)
                        .ApplyFilter(rejectedByMeFilter)
                        .ApplyFilter(useCurrentMonthForEndDistributionDateFactFilter)
                        .ApplyFilter(dummyAdvertisementsFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(selectExpression)
                        .Distinct()
                        .AsEnumerable()
                        .Select(x => ListDataSelectFunc(x, _userIdentifierService, UserContext));
                case EntityName.LegalPerson:
                    return query
                        .Where(x => x.LegalPersonId == filterManager.ParentEntityId)
                        .ApplyFilter(forNextEditionFilter)
                        .ApplyFilter(forNextMonthEditionFilter)
                        .ApplyFilter(withoutAdvertisementFilter)
                        .ApplyFilter(rejectedByMeFilter)
                        .ApplyFilter(useCurrentMonthForEndDistributionDateFactFilter)
                        .ApplyFilter(dummyAdvertisementsFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(selectExpression)
                        .Distinct()
                        .AsEnumerable()
                        .Select(x => ListDataSelectFunc(x, _userIdentifierService, UserContext));
                case EntityName.Account:
                    return query
                        .Where(x => x.AccountId == filterManager.ParentEntityId)
                        .ApplyFilter(forNextEditionFilter)
                        .ApplyFilter(forNextMonthEditionFilter)
                        .ApplyFilter(withoutAdvertisementFilter)
                        .ApplyFilter(rejectedByMeFilter)
                        .ApplyFilter(useCurrentMonthForEndDistributionDateFactFilter)
                        .ApplyFilter(dummyAdvertisementsFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(selectExpression)
                        .Distinct()
                        .AsEnumerable()
                        .Select(x => ListDataSelectFunc(x, _userIdentifierService, UserContext));
                case EntityName.Firm:
                    return query
                        .Where(x => x.FirmId == filterManager.ParentEntityId)
                        .ApplyFilter(forNextEditionFilter)
                        .ApplyFilter(forNextMonthEditionFilter)
                        .ApplyFilter(withoutAdvertisementFilter)
                        .ApplyFilter(rejectedByMeFilter)
                        .ApplyFilter(useCurrentMonthForEndDistributionDateFactFilter)
                        .ApplyFilter(dummyAdvertisementsFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(selectExpression)
                        .Distinct()
                        .AsEnumerable()
                        .Select(x => ListDataSelectFunc(x, _userIdentifierService, UserContext));
                default:
                    return query
                        .ApplyFilter(forNextEditionFilter)
                        .ApplyFilter(forNextMonthEditionFilter)
                        .ApplyFilter(withoutAdvertisementFilter)
                        .ApplyFilter(rejectedByMeFilter)
                        .ApplyFilter(useCurrentMonthForEndDistributionDateFactFilter)
                        .ApplyFilter(dummyAdvertisementsFilter)
                        .ApplyQuerySettings(querySettings, out count)
                        .Select(selectExpression)
                        .Distinct()
                        .AsEnumerable()
                        .Select(x => ListDataSelectFunc(x, _userIdentifierService, UserContext));
            }
        }
    }
}