using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class CouponPeriodOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IQuery _query;

        public CouponPeriodOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            const int PeriodLengthInDays = 4;

            var periodStart = ruleContext.ValidationParams.IsMassValidation 
                                ? ruleContext.ValidationParams.Mass.Period.Start 
                                : ruleContext.ValidationParams.Single.Period.Start;

            var periodEnd = ruleContext.ValidationParams.IsMassValidation
                                ? ruleContext.ValidationParams.Mass.Period.End
                                : ruleContext.ValidationParams.Single.Period.End;

            var badAdvertisemements =
                _query.For<Order>()
                       .Where(ruleContext.OrdersFilterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                       .SelectMany(orderPosition =>
                                   orderPosition.OrderPositionAdvertisements
                                                .Where(opa => opa.Advertisement != null)
                                                .SelectMany(
                                                    opa => opa.Advertisement.AdvertisementElements
                                                              .Where(x => !x.IsDeleted &&
                                                                          x.BeginDate != null &&
                                                                          x.EndDate != null &&
                                                                          (DbFunctions.DiffDays(x.BeginDate, x.EndDate) < PeriodLengthInDays ||
                                                                           (ruleContext.ValidationParams.IsMassValidation &&
                                                                            DbFunctions.DiffDays(periodStart, x.EndDate) < PeriodLengthInDays) ||
                                                                           (ruleContext.ValidationParams.IsMassValidation &&
                                                                            DbFunctions.DiffDays(x.BeginDate, periodEnd) < PeriodLengthInDays)))
                                                              .Select(advertisement => new
                                                                  {
                                                                      OrderPositionId = orderPosition.Id,
                                                                      OrderPositionName = opa.Position.Name,
                                                                      OrderId = orderPosition.Order.Id,
                                                                      OrderNumber = orderPosition.Order.Number,
                                                                      AdvertisementId = advertisement.Id,
                                                                      AdvertisementName = advertisement.Advertisement.Name,
                                                                      AdvertisementElementId = advertisement.Id
                                                                  })))
                       .ToArray();

            return badAdvertisemements.Select(x =>
                                              new OrderValidationMessage
                                                  {
                                                      Type = ruleContext.ValidationParams.IsMassValidation ? MessageType.Error : MessageType.Warning,
                                                      OrderId = x.OrderId,
                                                      OrderNumber = x.OrderNumber,
                                                      MessageText = !ruleContext.ValidationParams.IsMassValidation
                                                                        ? BLResources.AdvertisementPeriodError
                                                                        : string.Format(
                                                                                        BLResources.AdvertisementPeriodEndsBeforeReleasePeriodBegins,
                                                                                        GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.AdvertisementElement(), x.AdvertisementName, x.AdvertisementElementId),
                                                                                        GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.OrderPosition(), x.OrderPositionName, x.OrderPositionId))
                                                  });
        }
    }
}
