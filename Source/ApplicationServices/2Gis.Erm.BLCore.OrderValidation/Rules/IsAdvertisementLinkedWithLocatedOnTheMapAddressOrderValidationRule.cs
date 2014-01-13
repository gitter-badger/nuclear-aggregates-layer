using System;
using System.Collections.Generic;
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
    /// <summary>
    /// В ERM дорабатываем запрет возможности продажи на пустой адрес.
    /// Необходимо дополнительный атруб от IR по которому мы будем определять что адрес пустой.
    /// Отображаться в списке доступных объектов привязки данный адрес не должен.
    /// </summary>
    public sealed class IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var badAdvertisemements =
                _finder.Find(filterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive
                                               && !orderPosition.IsDeleted)
                       .SelectMany(
                           orderPosition =>
                           orderPosition.OrderPositionAdvertisements.Where(opa => !opa.FirmAddress.IsLocatedOnTheMap
                                                                                  && opa.FirmAddress.Firm.OrganizationUnit.InfoRussiaLaunchDate != null
                                                                                  && opa.Position.CategoryId != 11
                                                                                  && opa.Position.CategoryId != 14
                                                                                  && opa.Position.CategoryId != 26)
                                        .Select(advertisement => new
                                            {
                                                OrderPositionId = orderPosition.Id,
                                                OrderPositionName = advertisement.Position.Name,
                                                OrderId = orderPosition.Order.Id,
                                                OrderNumber = orderPosition.Order.Number,
                                                FirmAddressId = advertisement.FirmAddress.Id,
                                                FirmAddressName = advertisement.FirmAddress.Address,
                                            }))
                       .ToArray();

            foreach (var advertisemement in badAdvertisemements)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, advertisemement.OrderPositionName, advertisemement.OrderPositionId);
                var addressDescription = GenerateDescription(EntityName.FirmAddress, advertisemement.FirmAddressName, advertisemement.FirmAddressId);
                messages.Add(
                    new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = advertisemement.OrderId,
                            OrderNumber = advertisemement.OrderNumber,
                            MessageText = string.Format(BLResources.AdvertisementIsLinkedWithEmptyAddressError, orderPositionDescription, addressDescription)
                        });
            }
        }
    }
}
