using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class CouponPeriodOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public CouponPeriodOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            const int PeriodLengthInDays = 4;

            var badAdvertisemements =
                _finder.Find(filterPredicate)
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
                                                                           (IsCheckMassive &&
                                                                            DbFunctions.DiffDays(request.Period.Start, x.EndDate) < PeriodLengthInDays) ||
                                                                           (IsCheckMassive &&
                                                                            DbFunctions.DiffDays(x.BeginDate, request.Period.End) < PeriodLengthInDays)))
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

            foreach (var advertisemement in badAdvertisemements)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, advertisemement.OrderPositionName, advertisemement.OrderPositionId);
                var elementDescription = GenerateDescription(EntityName.AdvertisementElement, advertisemement.AdvertisementName, advertisemement.AdvertisementElementId);

                messages.Add(new OrderValidationMessage
                    {
                        Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                        OrderId = advertisemement.OrderId,
                        OrderNumber = advertisemement.OrderNumber,
                        MessageText = !IsCheckMassive
                                          ? BLResources.AdvertisementPeriodError
                                          : string.Format(
                                              BLResources.AdvertisementPeriodEndsBeforeReleasePeriodBegins, elementDescription, orderPositionDescription)
                    });
            }
        }
    }
}
