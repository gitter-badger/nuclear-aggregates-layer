using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Dto;
using DoubleGis.Erm.BLCore.API.MoDi.Enums;
using DoubleGis.Erm.BLCore.MoDi.Configurations;

namespace DoubleGis.Erm.BLCore.MoDi
{
    public sealed class MoneyDistributionCalculator
    {
        private readonly ESalesPointType _source;
        private readonly ESalesPointType _dest;
        private readonly SaleSchema _saleSchema;
        private readonly bool _isBeforeFirstApril;

        public MoneyDistributionCalculator(ContributionTypeEnum source, ContributionTypeEnum dest, bool isSelfSale, bool isBeforeFirstApril)
        {
            _source = (source == ContributionTypeEnum.Branch) ? ESalesPointType.Filial : (source == ContributionTypeEnum.Franchisees) ? ESalesPointType.Franch : ESalesPointType.None;
            _dest = (dest == ContributionTypeEnum.Branch) ? ESalesPointType.Filial : (dest == ContributionTypeEnum.Franchisees) ? ESalesPointType.Franch : ESalesPointType.None;

            _isBeforeFirstApril = isBeforeFirstApril;

            // единственное место где может появляться self
            var saleSchemaDest = isSelfSale ? ESalesPointType.Self : _dest;
            _saleSchema = SaleSchemasSettings.Instance.SaleSchemas.Cast<SaleSchema>().FirstOrDefault(x => x.Source == _source && x.Dest == saleSchemaDest);
        }

        public IEnumerable<PlatformCost> CalculatePlatformCosts(OrderPositionDto orderPositionDto)
        {
            OrderPositionDto[] orderPositionDtos;

            var packagePosition = PackagePositionsSettings.Instance.PackagePositions.Cast<PackagePosition>().FirstOrDefault(y => y.Id == orderPositionDto.PositionId && y.PriceId == orderPositionDto.PriceId);
            if (packagePosition != null)
            {
                var childPositionsCost = packagePosition.ChildPositions.Cast<ConfigPosition>().Sum(x => x.Cost);

                // разворачиваем пакетную позицию на простые
                orderPositionDtos = packagePosition.ChildPositions.Cast<ConfigPosition>().Select(y =>
                {
                    // скидку распределяем по пакету пропорционально стоимости позиций
                    var childDiscountSum = orderPositionDto.CalculateDiscountViaPercent ?
                        (y.Cost * orderPositionDto.DiscountPercent) / 100m :
                        (y.Cost * orderPositionDto.DiscountSum) / childPositionsCost;

                    return new OrderPositionDto
                    {
                        PositionId = y.Id,
                        PriceId = orderPositionDto.PriceId,

                        Amount = orderPositionDto.Amount,
                        CalculateDiscountViaPercent = orderPositionDto.CalculateDiscountViaPercent,
                        DiscountSum = childDiscountSum,
                        DiscountPercent = orderPositionDto.DiscountPercent,
                        CategoryRate = orderPositionDto.CategoryRate,

                        PricePositionCost = y.Cost,
                        ExtendedPlatform = y.Platform,
                    };

                }).ToArray();
            }
            else
            {
                // FIXME {a.tukaev, 02.09.2013}: Временно вернул логику с продакшна; нужно решить вопрос с заказами с BeginDistributionDate > 1.04, оформелнными на прайс с BeginDate < 1.04
                if (orderPositionDto.ExtendedPlatform == PlatformsExtended.None)
                {
                    throw new ApplicationException(string.Format("Для позиции номенклатуры id='{0}' и позиции прайса id='{1}' не найдены настройки в конфигурационном файле", orderPositionDto.PositionId, orderPositionDto.PriceId));
                }

                orderPositionDtos = new[] { orderPositionDto };
            }

            var positionDistributions = orderPositionDtos.Select(CalculateDistribution).ToArray();
            return positionDistributions;
        }

        private PlatformCost CalculateDistribution(OrderPositionDto orderPositionDto)
        {
            // Учитываем коэффициент домножения для коэффициентозависимой позиции (для независимой - 1)
            var rateForPosition = orderPositionDto.CategoryRate;

            decimal pricePerUnit;
            decimal pricePerUnitWithVat;

            // calculate PricePerUnit
            switch (_source)
            {
                case ESalesPointType.Franch:
                    {
                        switch (_dest)
                        {
                            case ESalesPointType.Franch:
                                {
                                    var pricePerUnitExact = orderPositionDto.PricePositionCost * rateForPosition;

                                    pricePerUnit = Math.Round(pricePerUnitExact, 2, MidpointRounding.ToEven);
                                    pricePerUnitWithVat = pricePerUnit;
                                }
                                break;
                            case ESalesPointType.Filial:
                                {
                                    var pricePerUnitExact = orderPositionDto.PricePositionCost * rateForPosition * 1.18m;

                                    pricePerUnit = Math.Round(pricePerUnitExact, 2, MidpointRounding.ToEven);
                                    pricePerUnitWithVat = pricePerUnit;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;

                case ESalesPointType.Filial:
                    {
                        var pricePerUnitExact = orderPositionDto.PricePositionCost * rateForPosition;
                        pricePerUnit = Math.Round(pricePerUnitExact, 2, MidpointRounding.ToEven);

                        var pricePerUnitWithVatExact = pricePerUnitExact * 1.18m;
                        pricePerUnitWithVat = Math.Round(pricePerUnitWithVatExact, 2, MidpointRounding.ToEven);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // ShipmentPlan == Amount, т.к. расчёт идёт за 1 месяц
            var payablePrice = pricePerUnit * orderPositionDto.Amount;
            var payablePriceWithVat = pricePerUnitWithVat * orderPositionDto.Amount;

            var exactDiscountSum = orderPositionDto.CalculateDiscountViaPercent ? (payablePriceWithVat * orderPositionDto.DiscountPercent) / 100m : orderPositionDto.DiscountSum;
            var exactDiscountPercent = payablePriceWithVat == 0 ? 0 : (exactDiscountSum * 100m) / payablePriceWithVat;

            var discountSum = Math.Round(exactDiscountSum, 2, MidpointRounding.ToEven);
            var payablePlan = payablePriceWithVat - discountSum;

            if (_isBeforeFirstApril &&
                orderPositionDto.ExtendedPlatform == PlatformsExtended.Mobile &&
                _source == ESalesPointType.Franch &&
                _dest == ESalesPointType.Filial)
            {
                payablePlan *= 1.18m;
            }

            var platformCost = new PlatformCost
            {
                PositionId = orderPositionDto.PositionId,
                From = _source,
                To = _dest,
                Platform = orderPositionDto.ExtendedPlatform,
                PayablePlan = payablePlan,
                PayablePrice = payablePrice,
            };

            var saleSchemaPlatform = _saleSchema != null ? _saleSchema.Platforms.Cast<Configurations.Platform>().FirstOrDefault(y => y.Id == orderPositionDto.ExtendedPlatform) : null;
            if (saleSchemaPlatform != null)
            {
                var key = Tuple.Create(_saleSchema.Source, _saleSchema.Dest, saleSchemaPlatform.Id);

                Func<decimal, decimal, decimal> formula;
                if (!FormulaMapAfterFirstApril.TryGetValue(key, out formula))
                {
                    throw new ApplicationException(string.Format("Не найдена формула для распределения между {0} и {1} для платформы {2}", _saleSchema.Source, _saleSchema.Dest, saleSchemaPlatform.Id));
                }

                if (_isBeforeFirstApril)
                {
                    Func<decimal, decimal, decimal> formula2;
                    if (FormulaMapBeforeFirstApril.TryGetValue(key, out formula2))
                    {
                        formula = formula2;
                    }
                }

                var cost = formula(payablePrice, exactDiscountPercent);

                platformCost.Cost = Math.Round(cost, 2, MidpointRounding.ToEven);
                platformCost.DiscountCost = Math.Round(cost * saleSchemaPlatform.Multiplier, 2, MidpointRounding.ToEven);

                platformCost.NewTo = saleSchemaPlatform.Direction;
            }
            else
            {
                platformCost.NewTo = platformCost.To;
            }

            return platformCost;
        }

        #region formulas

        private static readonly Dictionary<Tuple<ESalesPointType, ESalesPointType, PlatformsExtended>, Func<decimal, decimal, decimal>> FormulaMapBeforeFirstApril = new Dictionary<Tuple<ESalesPointType, ESalesPointType, PlatformsExtended>, Func<decimal, decimal, decimal>>
        {
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.Mobile),
                (payablePrice, discountPercent) => (payablePrice * (1 - discountPercent / 100)) * 1.18m
            },
        };

        private static readonly Dictionary<Tuple<ESalesPointType, ESalesPointType, PlatformsExtended>, Func<decimal, decimal, decimal>> FormulaMapAfterFirstApril = new Dictionary<Tuple<ESalesPointType, ESalesPointType, PlatformsExtended>, Func<decimal, decimal, decimal>>
        {
            // Filial to Franch
            {
                Tuple.Create(ESalesPointType.Filial, ESalesPointType.Franch, PlatformsExtended.Desktop),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Filial, ESalesPointType.Franch, PlatformsExtended.Mobile),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },

            // Franch to Self
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Self, PlatformsExtended.ApiOnline),
                (payablePrice, discountPercent) => payablePrice 
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Self, PlatformsExtended.ApiPartner),
                (payablePrice, discountPercent) => payablePrice
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Self, PlatformsExtended.Online),
                (payablePrice, discountPercent) => payablePrice
            },

            // Franch to Franch
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Franch, PlatformsExtended.Desktop),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Franch, PlatformsExtended.Mobile),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Franch, PlatformsExtended.ApiOnline),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Franch, PlatformsExtended.ApiPartner),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Franch, PlatformsExtended.Online),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },

            // Franch to Filial
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.Desktop),
                (payablePrice, discountPercent) => (payablePrice * (1 - discountPercent / 100)) * 1.18m
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.Mobile),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.ApiOnline),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.ApiPartner),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
            {
                Tuple.Create(ESalesPointType.Franch, ESalesPointType.Filial, PlatformsExtended.Online),
                (payablePrice, discountPercent) => payablePrice * (1 - discountPercent / 100)
            },
        };

        #endregion
    }
}