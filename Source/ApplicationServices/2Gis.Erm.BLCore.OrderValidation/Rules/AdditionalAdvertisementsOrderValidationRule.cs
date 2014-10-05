﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class AdditionalAdvertisementsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;

        private readonly PositionCategoryDto[] _positionCategories =
        {
            new PositionCategoryDto
            {
                // Микрокомментарий в рубрике
                PrimaryPositionCategoryIds = new[] { 30L, 40L },
                SecondaryPositionCategoryId = 25,
                AdditionalPositionCategoryName = BLResources.AdditionalTextForMicrocomment,
            },
            new PositionCategoryDto
            {
                // Баннер в рубрике
                PrimaryPositionCategoryIds = new[] { 23L, 2L },
                SecondaryPositionCategoryId = 3,
                AdditionalPositionCategoryName = "Дополнительный макет для баннера",
            }
        };

        public AdditionalAdvertisementsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            Expression<Func<Order, bool>> filterPredicate = ruleContext.OrdersFilterPredicate;
            long? firmId = null;
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                long organizationUnitId;
                filterPredicate = GetFilterPredicateToGetLinkedOrders(_finder, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmId);
            }

            foreach (var positionCategory in _positionCategories)
            {
                var primaryOrderPositions =
                    _finder.Find(filterPredicate)
                           .SelectMany(x => x.OrderPositions
                                             .Where(y => y.IsActive && !y.IsDeleted &&
                                                         (!firmId.HasValue || y.Order.FirmId == firmId.Value) &&
                                                         y.OrderPositionAdvertisements.Any(
                                                                                           z => positionCategory.PrimaryPositionCategoryIds.Contains(z.Position.CategoryId)))
                                             .Select(y => new
                                                              {
                                                                  y.Id,
                                                                  FirmId = y.Order.FirmId,
                                                                  OrderId = y.OrderId,
                                                                  OrderNumber = y.Order.Number,
                                                                  AdvertisementIds = y.OrderPositionAdvertisements
                                                                                      .Where(z => positionCategory.PrimaryPositionCategoryIds.Contains(z.Position.CategoryId))
                                                                                      .Select(z => z.AdvertisementId)
                                                                                      .Distinct(),
                                                              })
                                             .Where(y => y.AdvertisementIds.Any()))
                           .ToArray();

                if (!primaryOrderPositions.Any())
                {
                    continue;
                }

                var primaryOrderPositionsWithSharedAds = primaryOrderPositions
                    .Where(x => primaryOrderPositions.Any(y => y.Id != x.Id && y.FirmId == x.FirmId && y.AdvertisementIds.Except(x.AdvertisementIds).Any()))
                    .Select(x => new
                                     {
                                         x.Id,
                                         x.FirmId,
                                         x.OrderId,
                                         x.OrderNumber
                                     })
                    .ToArray();

                if (!primaryOrderPositionsWithSharedAds.Any())
                {
                    continue;
                }

                var ordersWithSecondaryPositions =
                    _finder.Find(filterPredicate)
                           .Select(x => new
                                            {
                                                x.Id,
                                                x.Number,
                                                Positions = x.OrderPositions
                                                             .Where(y => y.IsActive && !y.IsDeleted &&
                                                                         (!firmId.HasValue || y.Order.FirmId == firmId.Value))
                                                             .SelectMany(
                                                                         y =>
                                                                         y.OrderPositionAdvertisements.Where(
                                                                                                             z =>
                                                                                                             z.Position.CategoryId == positionCategory.SecondaryPositionCategoryId)),
                                                FirmId = x.FirmId
                                            })
                           .Where(x => x.Positions.Any())
                           .ToArray();

                var primaryOrderPositionsWithSharedAdsWithoutSecondaryPositions =
                    primaryOrderPositionsWithSharedAds.Where(x => ordersWithSecondaryPositions.All(y => y.FirmId != x.FirmId)).ToArray();

                if (!ruleContext.ValidationParams.IsMassValidation)
                {
                    var badItem = primaryOrderPositionsWithSharedAdsWithoutSecondaryPositions.FirstOrDefault(x => x.OrderId == ruleContext.ValidationParams.Single.OrderId);
                    if (badItem != null)
                    {
                        return new[] 
                        {
                            new OrderValidationMessage
                                         {
                                             OrderId = badItem.OrderId,
                                             OrderNumber = badItem.OrderNumber,
                                             Type = MessageType.Error,
                                             MessageText = string.Format(BLResources.PositionRequiredTemplate, positionCategory.AdditionalPositionCategoryName),
                                         }
                        };
                    }
                }
                else
                {
                    PositionCategoryDto category = positionCategory;
                    return primaryOrderPositionsWithSharedAdsWithoutSecondaryPositions
                                .GroupBy(x => x.FirmId)
                                .Select(x => new OrderValidationMessage
                                                 {
                                                     OrderId = x.First().OrderId,
                                                     OrderNumber = x.First().OrderNumber,
                                                     Type = MessageType.Error,
                                                     MessageText = string.Format(BLResources.PositionRequiredTemplate, category.AdditionalPositionCategoryName),
                                                 });
                }
            }

            return Enumerable.Empty<OrderValidationMessage>();
        }

        private sealed class PositionCategoryDto
        {
            public long[] PrimaryPositionCategoryIds { get; set; }
            public long SecondaryPositionCategoryId { get; set; }

            // надо из базы вынести в локализацию
            public string AdditionalPositionCategoryName { get; set; }
        }
    }
}