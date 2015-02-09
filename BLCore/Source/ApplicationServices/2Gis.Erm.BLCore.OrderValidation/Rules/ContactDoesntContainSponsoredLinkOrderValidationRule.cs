using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверяем, если есть продажа позиции прайс-листа с категорией "Рекламная ссылка", то данные, которые занесены в качестве РМ (URL) не должен совпадать с URL указанным контактными данными (Фирма.Адрес.Контактные данные (Тип: Web-сайт)).
    /// </summary>
    public sealed class ContactDoesntContainSponsoredLinkOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public ContactDoesntContainSponsoredLinkOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
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
                                                                    .Where(y => y.ContactType == FirmAddressContactType.Website)
                                                       }))
                                        .Where(x => x.WebContacts.Any(y => x.AdvertisementLink.Contains(y.Contact))))
                       .ToArray();

            return badAdvertisemements.Select(a => 
                                                new OrderValidationMessage
                                                {
                                                    Type = MessageType.Warning,
                                                    OrderId = a.OrderId,
                                                    OrderNumber = a.OrderNumber,
                                                    MessageText =
                                                        string.Format(
                                                                        BLResources.FirmContactContainsSponsoredLinkError,
                                                                        GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Firm(), a.FirmName, a.FirmId),
                                                                        a.AdvertisementLink,
                                                                        GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.OrderPosition(), a.OrderPositionName, a.OrderPositionId),
                                                                        GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Order(), a.OrderNumber, a.OrderId))
                                                });
        }
    }
}
