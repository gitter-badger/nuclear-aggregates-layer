using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class SelfAdvertisementOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        // Самореклама только для ПК
        private const int SelfAdvertisementPositionCategoryId = 287;
        private static readonly long[] ValidPositionPlatforms = { (long)PlatformEnum.Desktop, (long)PlatformEnum.Independent };

        private readonly IFinder _finder;

        public SelfAdvertisementOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var predicate = filterPredicate;
            long? firmId = null;
            if (!IsCheckMassive)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentNullException("request.OrderId");
                }

                long organizationUnitId;
                predicate = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmId);
            }

            var orderGroupsToVerify = _finder.Find(predicate)
                                        .Where(x => !firmId.HasValue || x.FirmId == firmId.Value)
                                        .GroupBy(x => x.FirmId,
                                                 x => new
                                                     {
                                                         x.Id,
                                                         x.Number,

                                                         Positions = x.OrderPositions
                                                                      .Where(y => y.IsActive && !y.IsDeleted)
                                                                      .SelectMany(y => y.OrderPositionAdvertisements)
                                                                      .Select(y => new
                                                                          {
                                                                              PositionCategoryId = y.Position.CategoryId,
                                                                              PlatformDgppId = y.Position.Platform.DgppId,
                                                                          })
                                                                      .Distinct()
                                                     })
                                        .Where(x => x.SelectMany(y => y.Positions).Any(y => y.PositionCategoryId == SelfAdvertisementPositionCategoryId))
                                        .ToArray();

            foreach (var ordersGroup in orderGroupsToVerify)
            {
                var isOnlyValidPlatforms = ordersGroup.SelectMany(x => x.Positions).All(x => ValidPositionPlatforms.Contains(x.PlatformDgppId));
                if (isOnlyValidPlatforms)
                {
                    continue;
                }

                var selfAdvertisementOrder = ordersGroup.First(x => x.Positions.Any(y => y.PositionCategoryId == SelfAdvertisementPositionCategoryId));

                messages.Add(new OrderValidationMessage
                {
                    OrderId = selfAdvertisementOrder.Id,
                    OrderNumber = selfAdvertisementOrder.Number,
                    Type = MessageType.Error,
                    MessageText = BLResources.SelfAdvertisementOrderValidationRuleMessage,
                });
            }
        }
    }
}