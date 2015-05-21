using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class AdvertisementsOnlyWhiteListOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IQuery _query;

        public AdvertisementsOnlyWhiteListOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            // TODO {all, 10.10.2014}: попробовать избавиться от использования ruleContext.ValidationParams.Period, пока без конкретной привязки к массовой/единичной проверке - если там реализовать определение period для режима единичной проверки непосредственно в rule, то можно это свойство удалить из базового класса, оставив только в Mass*Paramsреализовав вариант с использованием Mass.Period, либо выводом period из свойств заказа для единичной валидации
            var orderInfos = _query.For<Order>()
                .Where(ruleContext.OrdersFilterPredicate)
                .Select(x => new
            {
                x.Id,
                x.Number,

                // проверка белого списка
                WhiteListPosition = 
                    new[] { x }
                    .Union(x.Firm.Orders.Where(y => y.Id != x.Id && !y.IsDeleted && y.IsActive)
                                        .Where(y => y.DestOrganizationUnitId == x.DestOrganizationUnitId)
                                        .Where(y => (y.WorkflowStepId == OrderState.OnApproval ||
                                                    y.WorkflowStepId == OrderState.Approved ||
                                                    y.WorkflowStepId == OrderState.OnTermination) &&
                                                    y.BeginDistributionDate <= ruleContext.ValidationParams.Period.Start &&
                                                    y.EndDistributionDateFact >= ruleContext.ValidationParams.Period.End))
                                        .SelectMany(y => y.OrderPositions)
                                        .Where(y => y.IsActive && !y.IsDeleted)
                                        .Select(y => new
                                            {
                                                WhiteListPositions = 
                                                    new[] { y.PricePosition.Position }
                                                        .Union(y.PricePosition.Position.ChildPositions.Where(z => z.IsActive && !z.IsDeleted).Select(z => z.ChildPosition))
                                                        .Distinct()
                                                        .Where(z => z.AdvertisementTemplateId != null && z.AdvertisementTemplate.IsAllowedToWhiteList)
                                                        .Select(z => new
                                                                {
                                                                    WhiteListAdvertisements = y.OrderPositionAdvertisements
                                                                                                    .Where(p => p.AdvertisementId != null)
                                                                                                    .Select(p => p.Advertisement)
                                                                                                    .Distinct()
                                                                                                    .Where(p => p.AdvertisementTemplateId == z.AdvertisementTemplateId.Value && p.IsSelectedToWhiteList)
                                                                                                    .Select(p => 
                                                                                                        new 
                                                                                                        {
                                                                                                            p.Id,
                                                                                                            p.Name,
                                                                                                        })
                                                                })
                                                        .Select(z => new
                                                        {
                                                            WhiteListAdsCount = z.WhiteListAdvertisements.Count(),
                                                            WhiteListAd = z.WhiteListAdvertisements.FirstOrDefault(),
                                                        })
                                            })
                                        .Where(y => y.WhiteListPositions.Any())
                                        .SelectMany(y => y.WhiteListPositions)
                                        .OrderByDescending(z => z.WhiteListAdsCount)
                                        .FirstOrDefault()
            }).ToArray();

            var results = new List<OrderValidationMessage>();

            foreach (var orderInfo in orderInfos)
            {
                if (orderInfo.WhiteListPosition != null)
                {
                    var whiteListPosition = orderInfo.WhiteListPosition;

                    switch (whiteListPosition.WhiteListAdsCount)
                    {
                        case 0:
                            {
                                if (ruleContext.ValidationParams.IsMassValidation || ruleContext.ValidationParams.Single.OrderId == orderInfo.Id)
                                {
                                    results.Add(new OrderValidationMessage
                                    {
                                        Type = ruleContext.ValidationParams.Type == ValidationType.PreReleaseFinal ? MessageType.Error : MessageType.Warning,
                                        OrderId = orderInfo.Id,
                                        OrderNumber = orderInfo.Number,

                                        MessageText = BLResources.AdvertisementForWhitelistDoesNotSpecified,
                                    });
                                }
                            }

                            break;

                        case 1:
                            {
                                var advertisement = whiteListPosition.WhiteListAd;
                                var advertisementDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Advertisement(), advertisement.Name, advertisement.Id);

                                results.Add(new OrderValidationMessage
                                {
                                    Type = MessageType.Info,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,

                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.AdvertisementChoosenForWhitelist, advertisementDescription)
                                });
                            }

                            break;

                        default:
                            {
                                results.Add(new OrderValidationMessage
                                {
                                    Type = MessageType.Error,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,

                                    MessageText = BLResources.MoreThanOneAdvertisementChoosenForWhitelist,
                                });
                            }

                            break;
                    }                    
                }
            }

            return results;
        }
    }
}
