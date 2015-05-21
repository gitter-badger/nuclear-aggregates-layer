using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class SelfAdvertisementOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        // Самореклама только для ПК
        private const int SelfAdvertisementPositionCategoryId = 287;
        private static readonly long[] ValidPositionPlatforms = { (long)PlatformEnum.Desktop, (long)PlatformEnum.Independent };

        private readonly IQuery _query;

        public SelfAdvertisementOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var predicate = ruleContext.OrdersFilterPredicate;
            long? firmId = null;
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                long organizationUnitId;
                predicate = GetFilterPredicateToGetLinkedOrders(_query, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmId);
            }

            var orderGroupsToVerify = _query.For<Order>()
                                        .Where(predicate)
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

            return orderGroupsToVerify
                    .Where(o => !o.SelectMany(x => x.Positions).All(x => ValidPositionPlatforms.Contains(x.PlatformDgppId)))
                    .Select(o => o.First(x => x.Positions.Any(y => y.PositionCategoryId == SelfAdvertisementPositionCategoryId)))
                    .Select(x => new OrderValidationMessage
                                     {
                                         OrderId = x.Id,
                                         OrderNumber = x.Number,
                                         Type = MessageType.Error,
                                         MessageText = BLResources.SelfAdvertisementOrderValidationRuleMessage,
                                     });
        }
    }
}