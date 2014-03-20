using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверяем, если есть продажа позиции прайс-листа с категорией "Рекламная ссылка", то данные, которые занесены в качестве РМ (URL) не должен совпадать с URL указанным контактными данными (Фирма.Адрес.Контактные данные (Тип: Web-сайт)).
    /// </summary>
    public sealed class ContactDoesntContainSponsoredLinkOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public ContactDoesntContainSponsoredLinkOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var badAdvertisemements =
                _finder.Find(filterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                       .SelectMany(
                           orderPosition =>
                           orderPosition.OrderPositionAdvertisements.Where(opa => opa.Advertisement != null)
                                        .SelectMany(
                                            opa =>
                                            opa.Advertisement.AdvertisementElements.Where(
                                                x => x.IsDeleted == false && x.AdvertisementElementTemplate.IsAdvertisementLink)
                                               .Select(
                                                   advertisement =>
                                                   new
                                                       {
                                                           OrderPositionId = orderPosition.Id,
                                                           OrderPositionName = opa.Position.Name,
                                                           OrderId = orderPosition.Order.Id,
                                                           OrderNumber = orderPosition.Order.Number,
                                                           FirmName = orderPosition.Order.Firm.Name,
                                                           FirmId = orderPosition.Order.Firm.Id,
                                                           AdvertisementLink = advertisement.Text,
                                                           WebContacts =
                                                       orderPosition.Order.Firm.FirmAddresses.SelectMany(y => y.FirmContacts)
                                                                    .Where(y => y.ContactType == (int)FirmAddressContactType.Website)
                                                       }))
                                        .Where(x => x.WebContacts.Any(y => x.AdvertisementLink.Contains(y.Contact))))
                       .ToArray();

            foreach (var advertisemement in badAdvertisemements)
            {
                var firmDescription = GenerateDescription(EntityName.Firm, advertisemement.FirmName, advertisemement.FirmId);
                var orderDescription = GenerateDescription(EntityName.Order, advertisemement.OrderNumber, advertisemement.OrderId);
                var positionDescription = GenerateDescription(EntityName.OrderPosition, 
                                                              advertisemement.OrderPositionName, 
                                                              advertisemement.OrderPositionId);

                messages.Add(
                    new OrderValidationMessage
                        {
                            Type = MessageType.Warning,
                            OrderId = advertisemement.OrderId,
                            OrderNumber = advertisemement.OrderNumber,
                            MessageText =
                                string.Format(
                                    BLResources.FirmContactContainsSponsoredLinkError,
                                    firmDescription,
                                    advertisemement.AdvertisementLink,
                                    positionDescription,
                                    orderDescription)
                        });
            }
        }
    }
}
