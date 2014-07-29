using System;
using System.Collections.Generic;
using System.Globalization;
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
    public sealed class AdvertisementsOnlyWhiteListOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public AdvertisementsOnlyWhiteListOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orderInfos = _finder.Find(filterPredicate).Select(x => new
            {
                x.Id,
                x.Number,

                // проверка белого списка
                WhiteListPosition = new[] { x }.Union(x.Firm.Orders.Where(y => y.Id != x.Id && !y.IsDeleted && y.IsActive)
                                                           .Where(y => y.DestOrganizationUnitId == x.DestOrganizationUnitId)
                                                           .Where(y => (y.WorkflowStepId == (int)OrderState.OnApproval ||
                                                                      y.WorkflowStepId == (int)OrderState.Approved ||
                                                                      y.WorkflowStepId == (int)OrderState.OnTermination) &&
                                                                      y.BeginDistributionDate <= request.Period.Start &&
                                                                      y.EndDistributionDateFact >= request.Period.End))
                                    .SelectMany(y => y.OrderPositions).Where(y => y.IsActive && !y.IsDeleted).Select(y => new
                {
                    WhiteListPositions = new[] { y.PricePosition.Position }.Union(y.PricePosition.Position.ChildPositions.Where(z => z.IsActive && !z.IsDeleted).Select(z => z.ChildPosition))
                                            .Distinct().Where(z => z.AdvertisementTemplateId != null && z.AdvertisementTemplate.IsAllowedToWhiteList).Select(z => new
                    {
                        WhiteListAdvertisements = y.OrderPositionAdvertisements
                                                        .Where(p => p.AdvertisementId != null)
                                                        .Select(p => p.Advertisement)
                                                        .Distinct()
                                                        .Where(p => p.AdvertisementTemplateId == z.AdvertisementTemplateId.Value && p.IsSelectedToWhiteList)
                                                        .Select(p => new
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
                .FirstOrDefault(),
            }).ToArray();

            foreach (var orderInfo in orderInfos)
            {
                if (orderInfo.WhiteListPosition != null)
                {
                    var whiteListPosition = orderInfo.WhiteListPosition;

                    switch (whiteListPosition.WhiteListAdsCount)
                    {
                        case 0:
                            {
                                if (IsCheckMassive || request.OrderId == orderInfo.Id)
                                {
                                    messages.Add(new OrderValidationMessage
                                    {
                                        Type = request.Type == ValidationType.PreReleaseFinal ? MessageType.Error : MessageType.Warning,
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
                                var advertisementDescription = GenerateDescription(EntityName.Advertisement, advertisement.Name, advertisement.Id);

                                messages.Add(new OrderValidationMessage
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
                                messages.Add(new OrderValidationMessage
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
        }
    }
}
