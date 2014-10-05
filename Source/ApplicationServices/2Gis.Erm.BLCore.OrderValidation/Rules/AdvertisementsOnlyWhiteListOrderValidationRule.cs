﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class AdvertisementsOnlyWhiteListOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;

        public AdvertisementsOnlyWhiteListOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var orderInfos = _finder.Find(ruleContext.OrdersFilterPredicate).Select(x => new
            {
                x.Id,
                x.Number,

                // проверка белого списка
                WhiteListPosition = 
                    new[] { x }
                    .Union(x.Firm.Orders.Where(y => y.Id != x.Id && !y.IsDeleted && y.IsActive)
                                        .Where(y => y.DestOrganizationUnitId == x.DestOrganizationUnitId)
                                        .Where(y => (y.WorkflowStepId == (int)OrderState.OnApproval ||
                                                    y.WorkflowStepId == (int)OrderState.Approved ||
                                                    y.WorkflowStepId == (int)OrderState.OnTermination) &&
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
                                var advertisementDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityName.Advertisement, advertisement.Name, advertisement.Id);

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
