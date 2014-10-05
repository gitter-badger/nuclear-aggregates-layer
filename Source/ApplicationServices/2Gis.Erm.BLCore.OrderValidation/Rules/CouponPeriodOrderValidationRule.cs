﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class CouponPeriodOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;

        public CouponPeriodOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }
        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            const int PeriodLengthInDays = 4;

            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
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
                                                                            DbFunctions.DiffDays(ruleContext.ValidationParams.Mass.Period.Start, x.EndDate) < PeriodLengthInDays) ||
                                                                           (ruleContext.ValidationParams.IsMassValidation &&
                                                                            DbFunctions.DiffDays(x.BeginDate, ruleContext.ValidationParams.Mass.Period.End) < PeriodLengthInDays)))
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
                                                                                        GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityName.AdvertisementElement, x.AdvertisementName, x.AdvertisementElementId),
                                                                                        GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityName.OrderPosition, x.OrderPositionName, x.OrderPositionId))
                                                  });
        }
    }
}
