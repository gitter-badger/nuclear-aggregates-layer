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
    /// Максимальная длинна символов без разделителя для РМ с типом "текст" должна быть символов (Разделитель - " ", "-", "/"). Удостовериться, что у нас реализовано точно так. Иначе выдавать сообщение - "Длина символов без разделителя не может быть более 28". Возможно подумать, вынести параметр в общие настройки;
    /// Для РМ с типом "текст", пользователем не должно быть указано сочетания символов без пробела "\r", "\n", "\p", "\i". Иначе выдавать сообщение - "В тексте РМ не должны присутствовать сочетания символов: "\r", "\n".";
    /// Для РМ с типом "текст", пользователем не должно быть указано спец. символ "Неразрывный пробел". Иначе выдавать сообщение - "В тексте РМ не должен присутствовать спец. символов: "Неразрывный пробел"."/
    /// </summary>
    [Obsolete("Проверка отключена, т.к. больше нет возможности получить невалидный по ней ЭРМ")]
    public sealed class AdvertisementTextFormattingOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private const char NonBreakingSpaceChar = (char)160;
        private const string NonBreakingSpaceString = "&nbsp;";

        private readonly string[] _restrictedStrings = { @"\r", @"\n", @"\p", @"\i" };
        private readonly IFinder _finder;

        public AdvertisementTextFormattingOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var badAdvertisemements = _finder.Find(filterPredicate)
                                             .SelectMany(order => order.OrderPositions)
                                             .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                                             .SelectMany(orderPosition =>
                                                         orderPosition.OrderPositionAdvertisements
                                                                      .Where(opa => opa.Advertisement != null)
                                                                      .SelectMany(opa => opa.Advertisement.AdvertisementElements
                                                                                            .Where(x => x.IsDeleted == false &&
                                                                                                x.AdvertisementElementTemplate.RestrictionType ==
                                                                                                        (int)AdvertisementElementRestrictionType.Text &&
                                                                                                        x.Text != null))
                                                                      .Select(element => new
                                                                              {
                                                                                  OrderId = orderPosition.Order.Id,
                                                                                  OrderNumber = orderPosition.Order.Number,
                                                                              AdvertisementText = element.Text,
                                                                              MaxSymbolsInAWord = element.AdvertisementElementTemplate.MaxSymbolsInWord,
                                                                              AdvertisementName = element.Advertisement.Name,
                                                                              AdvertisementElementId = element.Id
                                                                              }))
                                             .ToArray();

            foreach (var advertisemement in badAdvertisemements)
            {
                var advertisementElementDescription = GenerateDescription(EntityName.AdvertisementElement,
                                                              advertisemement.AdvertisementName,
                                                              advertisemement.AdvertisementElementId);

                if (advertisemement.AdvertisementText.Contains(NonBreakingSpaceChar.ToString()) ||
                    advertisemement.AdvertisementText.Contains(NonBreakingSpaceString))
                {
                    messages.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = advertisemement.OrderId,
                            OrderNumber = advertisemement.OrderNumber,
                            MessageText = string.Format(BLResources.NonBreakingSpaceError, advertisementElementDescription)
                        });
                }

                foreach (var restrictedString in _restrictedStrings)
                {
                    if (advertisemement.AdvertisementText.ToLower().Contains(restrictedString))
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = advertisemement.OrderId,
                                OrderNumber = advertisemement.OrderNumber,
                                MessageText = string.Format(BLResources.RestrictedSymbolsInAdvertisementElementText,
                                                            advertisementElementDescription,
                                                            restrictedString)
                            });
                    }
                }
            }
        }
    }
}
