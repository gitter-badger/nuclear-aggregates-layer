using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
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
    public sealed class AdvertisementsWithoutWhiteListOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IQuery _query;

        public AdvertisementsWithoutWhiteListOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var orderInfos = _query.For<Order>()
                .Where(ruleContext.OrdersFilterPredicate)
                .Select(x => new
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
                                     && (!y.PricePosition.Position.DgppId.HasValue || y.PricePosition.Position.DgppId.Value != PositionTools.AdditionalPackageDgppId),

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
                            ElementIsInvalid = (bool?)(p.AdvertisementElementTemplate.NeedsValidation && (p.AdvertisementElementStatus.Status == (int)AdvertisementElementStatusValue.Invalid) && p.Advertisement.FirmId != null),

                            ElementIsDraft = (bool?)(p.AdvertisementElementTemplate.NeedsValidation && (p.AdvertisementElementStatus.Status == (int)AdvertisementElementStatusValue.Draft) && p.Advertisement.FirmId != null),
                        })
                    }),
                }),
            }).ToArray();

            var results = new List<OrderValidationMessage>();

            foreach (var orderInfo in orderInfos)
            {
                // order positions fails
                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    var orderPositionDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.OrderPosition(), orderPosition.PositionName, orderPosition.Id);

                    // position fails
                    foreach (var positionFail in orderPosition.RequiredPositionFails)
                    {
                        if (positionFail.AdvertisementIsRequired)
                        {
                            var messageText = (orderPosition.PositionId == positionFail.Id) ?
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckPositionMustHaveAdvertisements, orderPositionDescription) :
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckCompositePositionMustHaveAdvertisements, orderPositionDescription, positionFail.Name);

                            results.Add(new OrderValidationMessage
                            {
                                Type = (ruleContext.ValidationParams.IsMassValidation || orderInfo.IsRegionalOrder) ? MessageType.Error : MessageType.Warning,
                                OrderId = orderInfo.Id,
                                OrderNumber = orderInfo.Number,

                                MessageText = messageText,
                            });
                        }
                        else if (positionFail.OpaIsEmpty)
                        {
                            var messageText =
                                string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckCompositePositionMustHaveLinkingObject, orderPositionDescription, positionFail.Name);

                            results.Add(new OrderValidationMessage
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
                        var advertisementDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Advertisement(), advertisementFail.Name, advertisementFail.Id);

                        if (advertisementFail.AdvertisementIsDeleted)
                        {
                            results.Add(new OrderValidationMessage
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
                            var firmDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Firm(), orderInfo.FirmName, orderInfo.FirmId);

                            results.Add(new OrderValidationMessage
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
                            var elementDescription = GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.AdvertisementElement(), elementFail.Name, elementFail.Id);

                            if (elementFail.ElementIsRequired)
                            {
                                results.Add(new OrderValidationMessage
                                {
                                    Type = (ruleContext.ValidationParams.IsMassValidation || orderInfo.IsRegionalOrder) ? MessageType.Error : MessageType.Warning,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,

                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrdersCheckPositionMustHaveAdvertisementElements, advertisementDescription, elementDescription)
                                });
                            }

                            if (elementFail.ElementIsInvalid == true)
                            {
                                results.Add(new OrderValidationMessage
                                {
                                    Type = ruleContext.ValidationParams.IsMassValidation ? MessageType.Error : MessageType.Warning,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,
                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrdersCheckAdvertisementElementWasInvalidated, advertisementDescription, elementDescription)
                                });
                            }

                            if (elementFail.ElementIsDraft == true)
                            {
                                results.Add(new OrderValidationMessage
                                {
                                    Type = ruleContext.ValidationParams.IsMassValidation ? MessageType.Error : MessageType.Warning,
                                    OrderId = orderInfo.Id,
                                    OrderNumber = orderInfo.Number,
                                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrdersCheckAdvertisementElementIsDraft, advertisementDescription, elementDescription)
                                });
                            }
                        }
                    }                    
                }
            }

            return results;
        }
    }
}
