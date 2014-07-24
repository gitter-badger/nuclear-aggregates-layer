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
    public sealed class AdvertisementsWithoutWhiteListOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public AdvertisementsWithoutWhiteListOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            const int AdditionalPackageDgppId = 11572; // ДгппИд элемента номенклатуры "пакет "Дополнительный"" нужен для костыля-исключения на 2+2 месяца (до Июля)

            var orderInfos = _finder.Find(filterPredicate).Select(x => new
            {
                x.Id,
                x.Number,

                x.FirmId,
                FirmName = x.Firm.Name,

                x.BeginDistributionDate,
                x.EndDistributionDatePlan,

                IsRegionalOrder = x.SourceOrganizationUnitId != x.DestOrganizationUnitId,

                OrderPositions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted).Select(y => new
                {
                    y.Id,
                    PositionId = y.PricePosition.Position.Id,
                    PositionName = y.PricePosition.Position.Name,

                    RequiredPositionFails = new[] { y.PricePosition.Position }.Union(y.PricePosition.Position.ChildPositions.Where(z => z.IsActive && !z.IsDeleted).Select(z => z.ChildPosition))
                                            .Where(z => z.AdvertisementTemplate.IsAdvertisementRequired).Select(z => new
                    {
                        z.Id,
                        z.Name,

                        OpaIsEmpty = y.OrderPositionAdvertisements.All(p => p.PositionId != z.Id)
                                     && (!y.PricePosition.Position.DgppId.HasValue || y.PricePosition.Position.DgppId.Value != AdditionalPackageDgppId),

                        AdvertisementIsRequired = 
                        y.OrderPositionAdvertisements.Where(p => p.PositionId == z.Id).Any(p => p.AdvertisementId == null),
                    }),

                    AdvertisementFails = y.OrderPositionAdvertisements.Where(z => z.AdvertisementId != null).Select(z => z.Advertisement).Distinct().Select(z => new
                    {
                        z.Id,
                        z.Name,
                        z.FirmId,
                        AdvertisementIsDeleted = z.IsDeleted,
                        AdvertisementNotBelongsToFirm = x.Firm.Advertisements.All(p => p.Id != z.Id) && z.FirmId != null,
                        ElementFails = z.AdvertisementElements.Where(p => !p.IsDeleted).Select(p => new
                        {
                            p.Id,
                            p.AdvertisementElementTemplate.Name,

                            ElementIsRequired = p.AdvertisementElementTemplate.IsRequired && ((p.BeginDate == null || p.EndDate == null) && p.FileId == null && string.IsNullOrEmpty(p.Text)),
                            
                            // заглушки не верифицируем
                            ElementIsInvalid = (bool?)(p.AdvertisementElementTemplate.NeedsValidation && (p.Status == (int)AdvertisementElementStatus.Invalid) && p.Advertisement.FirmId != null),
                        })
                    }),
                }),
            }).ToArray();

            foreach (var orderInfo in orderInfos)
            {
                // order positions fails
                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, orderPosition.PositionName, orderPosition.Id);

                    // position fails
                    foreach (var positionFail in orderPosition.RequiredPositionFails)
                    {
                        if (positionFail.AdvertisementIsRequired)
                        {
                            var messageText = (orderPosition.PositionId == positionFail.Id) ?
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckPositionMustHaveAdvertisements, orderPositionDescription) :
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckCompositePositionMustHaveAdvertisements, orderPositionDescription, positionFail.Name);

                            messages.Add(new OrderValidationMessage
                            {
                                Type = (IsCheckMassive || orderInfo.IsRegionalOrder) ? MessageType.Error : MessageType.Warning,
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number,

                                MessageText = messageText,
                            });
                        }
                        else if (positionFail.OpaIsEmpty)
                        {
                            var messageText =
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckCompositePositionMustHaveLinkingObject, orderPositionDescription, positionFail.Name);

                            messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number,

                                MessageText = messageText,
                            });
                        }
                    }

                    foreach (var advertisementFail in orderPosition.AdvertisementFails)
                    {
                        var advertisementDescription = GenerateDescription(EntityName.Advertisement, advertisementFail.Name, advertisementFail.Id);

                        if (advertisementFail.AdvertisementIsDeleted)
                        {
                            messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number,

                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.RemovedAdvertisemendSpecifiedForPosition, orderPositionDescription, advertisementDescription)
                            });

                            continue;
                        }

                        if (advertisementFail.AdvertisementNotBelongsToFirm)
                        {
                            var firmDescription = GenerateDescription(EntityName.Firm, orderInfo.FirmName, orderInfo.FirmId);

                            messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number,

                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.AdvertisementSpecifiedForPositionDoesNotBelongToFirm, orderPositionDescription, advertisementDescription, firmDescription)
                            });

                            continue;
                        }

                        // element fails
                        foreach (var elementFail in advertisementFail.ElementFails)
                        {
                            var elementDescription = GenerateDescription(EntityName.AdvertisementElement, elementFail.Name, elementFail.Id);

                            if (elementFail.ElementIsRequired)
                            {
                                messages.Add(new OrderValidationMessage
                                {
                                    Type = (IsCheckMassive || orderInfo.IsRegionalOrder) ? MessageType.Error : MessageType.Warning,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,

                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrdersCheckPositionMustHaveAdvertisementElements, advertisementDescription, elementDescription)
                                });
                            }

                            if (elementFail.ElementIsInvalid == true)
                            {
                                messages.Add(new OrderValidationMessage
                                {
                                    Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,
                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrdersCheckAdvertisementElementWasInvalidated, advertisementDescription, elementDescription)
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
