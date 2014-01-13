using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    public sealed class OrderReadModel : IOrderReadModel
    {
        private readonly IFinder _finder;

        public OrderReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<OrderReleaseInfo> GetOrderReleaseInfos(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>())
                          .Select(o => new OrderReleaseInfo
                              {
                                  OrderId = o.Id,
                                  OrderNumber = o.Number,
                                  IsBudget = o.BudgetType == (int)OrderBudgetType.Budget,
                                  AccountId = o.AccountId
                                              ?? o.BranchOfficeOrganizationUnit.Accounts
                                                  .Where(x => x.IsActive && !x.IsDeleted && x.LegalPersonId == o.LegalPersonId)
                                                  .Select(x => (long?)x.Id)
                                                  .FirstOrDefault(),
                                  PriceId = o.OrderPositions.Select(p => p.PricePosition.PriceId).FirstOrDefault(),
                                  AmountToWithdrawSum = o.OrderReleaseTotals
                                                         .Where(x => x.ReleaseBeginDate == period.Start && x.ReleaseEndDate == period.End)
                                                         .Select(x => x.AmountToWithdraw)
                                                         .FirstOrDefault()
                              })
                          .ToArray();
        }

        public IEnumerable<Order> GetOrdersForRelease(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>());
        }

        public OrderValidationAdditionalInfo[] GetOrderValidationAdditionalInfos(IEnumerable<long> orderIds)
        {
            var orderInfos = _finder.Find(Specs.Find.ByIds<Order>(orderIds))
                                .Select(o => new
                                {
                                    o.Id,
                                    o.Number,
                                    o.OwnerCode,
                                    SourceOrganizationUnitName = o.SourceOrganizationUnit.Name,
                                    DestOrganizationUnitName = o.DestOrganizationUnit.Name,
                                    FirmName = o.Firm.Name,
                                    LegalPersonName = o.LegalPerson.ShortName
                                })
                .ToArray();

            // есть вариант дальнейшее вытягивание инфы по пользователю делать через ISecurityServiceUserIdentifier.GetUserInfo(int? userCode),
            // но т.к. это будет делаться последовательными запросами по одной записи пока от этого отказались
            var userInfos = _finder.Find(Specs.Find.ByIds<User>(orderInfos.Select(i => i.OwnerCode)))
                                .Select(u => new
                                {
                                    u.Id,
                                    u.DisplayName
                                });
            return orderInfos.GroupJoin(userInfos,
                                        orderInfo => orderInfo.OwnerCode,
                                        userInfo => userInfo.Id,
                                        (order, users) => new OrderValidationAdditionalInfo
                                        {
                                            Id = order.Id,
                                            Number = order.Number,
                                            SourceOrganizationUnitName = order.SourceOrganizationUnitName,
                                            DestOrganizationUnitName = order.DestOrganizationUnitName,
                                            FirmName = order.FirmName,
                                            LegalPersonName = order.LegalPersonName,
                                            OwnerName = users.Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                                        })
                .ToArray();
        }

        public IEnumerable<OrderInfo> GetOrderInfosForRelease(long organizationUnitId, TimePeriod period, int skipCount, int takeCount)
        {
            return _finder.Find<Order, OrderInfo>(OrderSpecs.Orders.Select.OrderInfosForRelease(),
                                                  OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>())
                          .OrderBy(o => o.Id)
                          .Skip(skipCount)
                          .Take(takeCount)
                          .ToArray();
        }

        public IEnumerable<Order> GetOrdersCompletelyReleasedBySourceOrganizationUnit(long sourceOrganizationUnitId)
        {
            return _finder
                        .Find(OrderSpecs.Orders.Find.CompletelyReleasedByOrganizationUnit(sourceOrganizationUnitId))
                        .ToArray();
        }
    }
}