using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public struct Satisfaction
    {
        public bool Satisfied { get; private set; }
        public bool SatisfiedOnce { get; private set; }
        public long FirstOrderId { get; private set; }
        public long FirstOrderPositionid { get; private set; }

        public void Satisfy(long orderId, long orderPositionId)
        {
            if (Satisfied)
            {
                if (orderId == FirstOrderId)
                {
                    return;
                }

                SatisfiedOnce = false;
                return;
            }

            Satisfied = true;
            SatisfiedOnce = true;
            FirstOrderId = orderId;
            FirstOrderPositionid = orderPositionId;
        }
    }

    public class ADPValidator
    {
        private readonly ADPValidationResultBuilder _validationResultBuilder;
        private readonly IDictionary<long, OrderStateStorage> _orderStates;
        private readonly IPriceConfigurationService _priceConfigurationService;

        private readonly IList<OrderPositionStateStorage> _orderPositionStates = new List<OrderPositionStateStorage>();
        private readonly IDictionary<long, List<OrderPositionStateStorage>> _pricePositionStatesByPosition = new Dictionary<long, List<OrderPositionStateStorage>>();

        public ADPValidator(
            ADPValidationResultBuilder validationResultBuilder,
            IDictionary<long, OrderStateStorage> orderStates,
            IPriceConfigurationService priceConfigurationService)
        {
            _validationResultBuilder = validationResultBuilder;
            _orderStates = orderStates;
            _priceConfigurationService = priceConfigurationService;
        }

        public void AddOrderPositionState(OrderPositionStateStorage positionData)
        {
            _orderPositionStates.Add(positionData);

            if (!_pricePositionStatesByPosition.ContainsKey(positionData.PositionId))
            {
                _pricePositionStatesByPosition[positionData.PositionId] = new List<OrderPositionStateStorage>();
            }

            _pricePositionStatesByPosition[positionData.PositionId].Add(positionData);
        }

        public void CheckSpecificOrder(long orderId)
        {
            CheckPrincipalPositions(orderId);
            CheckDeniedPositions(orderId);
        }

        public void CheckOrderBeingCancelled()
        {
            foreach (var orderData in _orderStates)
            {
                CheckPrincipalPositions(orderData.Key);
            }
        }

        public void CheckOrderBeingReapproved(long orderId)
        {
            CheckDeniedPositions(orderId);
        }

        public void MassiveCheckOrder()
        {
            foreach (var orderId in _orderStates.Keys)
            {
                CheckPrincipalPositions(orderId);
                CheckDeniedPositions(orderId);
            }
        }

        private void CheckPrincipalPositions(long orderId)
        {
            foreach (var pricePosition in _orderPositionStates.Where(item => item.Order.Id == orderId))
            {
                var priceConfigurationStorage = _priceConfigurationService.GetPriceConfigurationStorage(pricePosition.PriceId, pricePosition.PositionId);

                CheckPrincipalPositionsGroup(pricePosition, priceConfigurationStorage.PrincipalPositions);
            }
        }

        private void CheckPrincipalPositionsGroup(OrderPositionStateStorage positionData, IReadOnlyCollection<PricePositionDto.RelatedItemDto> group)
        {
            if (group.Count == 0)
            {
                return;
            }

            int periodBegin = positionData.BeginRelaseNumber;
            int periodLength = positionData.EndReleaseNumber - positionData.BeginRelaseNumber + 1;

            var principalPositionCoverage = new Satisfaction[periodLength];

            bool allLinkingObjectsWereDistributed = true;

            var matchItems = new List<PricePositionDto.RelatedItemDto>();
            var otherItems = new List<PricePositionDto.RelatedItemDto>();

            foreach (var relatedItem in group)
            {
                if (relatedItem.BindingCheckMode == ObjectBindingType.Match)
                {
                    matchItems.Add(relatedItem);
                }
                else
                {
                    otherItems.Add(relatedItem);
                }
            }

            if (matchItems.Any())
            {
                // Тогда мы для кажого объекта привязки должны найти соответствующий среди основных позиций
                var linkingObjects = positionData.LinkingObjects.ToArray();
                var linkingObjectReleases = new Satisfaction[linkingObjects.Length, periodLength];

                foreach (var relatedItem in matchItems)
                {
                    if (!_pricePositionStatesByPosition.ContainsKey(relatedItem.PositionId))
                    {
                        continue;
                    }

                    var principalPositions = _pricePositionStatesByPosition[relatedItem.PositionId];

                    if (positionData.Type == PositionType.Child)
                    {
                        principalPositions.RemoveAll(
                            x => x.Type == PositionType.Composite && x.OrderPositionId == positionData.ParentPositionStateStorage.OrderPositionId);
                    }

                    if (positionData.Type == PositionType.Composite)
                    {
                        principalPositions.RemoveAll(
                            x => x.Type == PositionType.Child && x.ParentPositionStateStorage.OrderPositionId == positionData.OrderPositionId);
                    }

                    foreach (var principalPosition in principalPositions)
                    {
                        var overlapBegin = Math.Max(positionData.BeginRelaseNumber, principalPosition.BeginRelaseNumber);
                        var overlapEnd = Math.Min(positionData.EndReleaseNumber, principalPosition.EndReleaseNumber);

                        for (var release = overlapBegin; release <= overlapEnd; release++)
                        {
                            principalPositionCoverage[release - periodBegin].Satisfy(principalPosition.Order.Id, principalPosition.OrderPositionId);
                        }

                        var apply = true;

                        if (principalPosition.Type == PositionType.Composite)
                        {
                            // If we are dealing with composite position and it does not contain all required linking objects, than we discard it at all,
                            // because we cannot use composite position as principal position partially
                            apply = linkingObjects.All(linkingObject => principalPosition.ContainsLinkingObjectLike(linkingObject));
                        }

                        if (!apply)
                        {
                            continue;
                        }

                        for (int linkingObjectIndex = 0; linkingObjectIndex < linkingObjects.Length; linkingObjectIndex++)
                        {
                            if (principalPosition.Type == PositionType.Composite || principalPosition.ContainsLinkingObjectLike(linkingObjects[linkingObjectIndex]))
                            {
                                for (var release = overlapBegin; release <= overlapEnd; release++)
                                {
                                    linkingObjectReleases[linkingObjectIndex, release - periodBegin].Satisfy(principalPosition.Order.Id,
                                                                                                             principalPosition.OrderPositionId);
                                }
                            }
                        }
                    }
                }

                foreach (Satisfaction item in linkingObjectReleases)
                {
                    if (!item.Satisfied)
                    {
                        allLinkingObjectsWereDistributed = false;
                    }
                    else if (item.SatisfiedOnce)
                    {
                        _validationResultBuilder.AddDependencyMessage(
                            item.FirstOrderId, 
                            item.FirstOrderPositionid,
                            positionData.Order.Id,
                            positionData.OrderPositionId);
                    }
                }
            }

            foreach (var relatedItem in otherItems)
            {
                if (_pricePositionStatesByPosition.ContainsKey(relatedItem.PositionId))
                {
                    var principalPositions = _pricePositionStatesByPosition[relatedItem.PositionId];
                    if (positionData.Type == PositionType.Child)
                    {
                        principalPositions.RemoveAll(
                            x => x.Type == PositionType.Composite && x.OrderPositionId == positionData.ParentPositionStateStorage.OrderPositionId);
                    }

                    if (positionData.Type == PositionType.Composite)
                    {
                        principalPositions.RemoveAll(
                            x => x.Type == PositionType.Child && x.ParentPositionStateStorage.OrderPositionId == positionData.OrderPositionId);
                    }

                    foreach (var principalPosition in principalPositions)
                    {
                        int overlapBegin = Math.Max(positionData.BeginRelaseNumber, principalPosition.BeginRelaseNumber);
                        int overlapEnd = Math.Min(positionData.EndReleaseNumber, principalPosition.EndReleaseNumber);

                        for (int release = overlapBegin; release <= overlapEnd; release++)
                        {
                            principalPositionCoverage[release - periodBegin].Satisfy(principalPosition.Order.Id, principalPosition.OrderPositionId);
                        }
                    }

                    if (relatedItem.BindingCheckMode == ObjectBindingType.Different)
                    {
                        // В этом случае, не должно быть совпадающих объектов привязки
                        var conflictPrincipalPositions = _pricePositionStatesByPosition[relatedItem.PositionId]
                            .Where(pricePositionData => positionData.LinkingObjects
                                                            .Any(pricePositionData.ContainsLinkingObjectLike))
                            .ToList();

                        if (positionData.Type == PositionType.Child)
                        {
                            conflictPrincipalPositions.RemoveAll(
                                x => x.Type == PositionType.Composite && x.OrderPositionId == positionData.ParentPositionStateStorage.OrderPositionId);
                        }

                        if (positionData.Type == PositionType.Composite)
                        {
                            conflictPrincipalPositions.RemoveAll(
                                x => x.Type == PositionType.Child && x.ParentPositionStateStorage.OrderPositionId == positionData.OrderPositionId);
                        }

                        foreach (var conflictPrincipalPosition in conflictPrincipalPositions)
                        {
                            string template = BLResources.ConflictingPrincipalPositionTemplate;

                            if (_validationResultBuilder.IsCheckSpecific)
                            {
                                string message = string.Format(template,
                                                               positionData.GetDescription(true),
                                                               conflictPrincipalPosition.GetDescription(true, positionData.Order != conflictPrincipalPosition.Order));
                                _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                            }

                            if (_validationResultBuilder.IsCheckMassive)
                            {
                                string message = string.Format(template,
                                                               positionData.GetDescription(false),
                                                               conflictPrincipalPosition.GetDescription(false, positionData.Order != conflictPrincipalPosition.Order));
                                _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                            }
                        }
                    }
                }
            }

            var atLeatOnePrincipalPosition = true;
            foreach (var item in principalPositionCoverage)
            {
                if (!item.Satisfied)
                {
                    atLeatOnePrincipalPosition = false;
                }
                else if (item.SatisfiedOnce)
                {
                    _validationResultBuilder.AddDependencyMessage(
                        item.FirstOrderId,
                        item.FirstOrderPositionid,
                        positionData.Order.Id,
                        positionData.OrderPositionId);
                }
            }

            if (!atLeatOnePrincipalPosition)
            {
                string template = BLResources.AssociatedPositionWithoutPrincipalTemplate;

                if (_validationResultBuilder.IsCheckSpecific)
                {
                    string message = string.Format(template, positionData.GetDescription(true));
                    _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                }

                if (_validationResultBuilder.IsCheckMassive)
                {
                    string message = string.Format(template, positionData.GetDescription(false));
                    _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                }
            }
            else
                if (matchItems.Any() && !allLinkingObjectsWereDistributed)
                {
                    string template = BLResources.LinkedObjectsMissedInPrincipals;

                    if (_validationResultBuilder.IsCheckSpecific)
                    {
                        string message = string.Format(template, positionData.GetDescription(true));
                        _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                    }

                    if (_validationResultBuilder.IsCheckMassive)
                    {
                        string message = string.Format(template, positionData.GetDescription(false));
                        _validationResultBuilder.AddMessage(MessageType.Error, message, positionData.Order.Id);
                    }
                }
        }

        private void CheckDeniedPositions(long orderId)
        {
            var compositePositionsWithErrors = new HashSet<Tuple<long, long>>();

            foreach (var pricePosition in _orderPositionStates.Where(item => item.Order.Id == orderId).OrderBy(item => item.Type))
            {
                // All positions that match the deinial rules
                var allSuitablePositions = new List<OrderPositionStateStorage>();

                var priceConfigurationStorage = _priceConfigurationService.GetPriceConfigurationStorage(pricePosition.PriceId, pricePosition.PositionId);
                foreach (var deniedPosition in priceConfigurationStorage.DeniedPositions)
                {
                    if (!_pricePositionStatesByPosition.ContainsKey(deniedPosition.PositionId))
                    {
                        continue;
                    }

                    var suitablePositions = _pricePositionStatesByPosition[deniedPosition.PositionId];
                    foreach (var suitablePosition in suitablePositions)
                    {
                        // We check composite positions first to skip children of those with errors
                        if (pricePosition.Type == PositionType.Child)
                        {
                            if (suitablePosition.Type == PositionType.Child)
                            {
                                if (compositePositionsWithErrors.Contains(new Tuple<long, long>(pricePosition.ParentPositionStateStorage.PositionId,
                                                                                              suitablePosition.ParentPositionStateStorage.PositionId)))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (compositePositionsWithErrors.Contains(new Tuple<long, long>(pricePosition.ParentPositionStateStorage.PositionId,
                                                                                              suitablePosition.PositionId)))
                                {
                                    continue;
                                }
                            }
                        }

                        if (deniedPosition.BindingCheckMode == ObjectBindingType.Match)
                        {
                            var haveCommonLinkingObjects = pricePosition.LinkingObjects.Any(item => suitablePosition.ContainsLinkingObjectLike(item));
                            if (!haveCommonLinkingObjects)
                            {
                                continue;
                            }
                        }

                        if (deniedPosition.BindingCheckMode == ObjectBindingType.Different && AllLinkingObjectsAreSame(pricePosition, suitablePosition))
                        {
                            continue;
                        }

                        if (suitablePosition.Order.Id == orderId &&
                            suitablePosition.OrderPositionAdvertisementId <= pricePosition.OrderPositionAdvertisementId)
                        {
                            // All denial rules are symmetrical so we don't want to output the same error twice
                            continue;
                        }

                        // Пакет не может быть запрещен своей подпозиции
                        if (suitablePosition.Type == PositionType.Child && suitablePosition.ParentPositionStateStorage.OrderPositionId == pricePosition.OrderPositionId)
                        {
                            continue;
                        }

                        if (pricePosition.Type == PositionType.Child && pricePosition.ParentPositionStateStorage.OrderPositionId == suitablePosition.OrderPositionId)
                        {
                            continue;
                        }


                        // Remember the position has an error to skip its children
                        if (pricePosition.Type != PositionType.Child)
                        {
                            if (suitablePosition.Type != PositionType.Child)
                            {
                                compositePositionsWithErrors.Add(new Tuple<long, long>(pricePosition.PositionId,
                                                                                     suitablePosition.PositionId));
                            }
                        }

                        allSuitablePositions.Add(suitablePosition);
                    }
                }

                // Again, the composite positions go first
                foreach (var suitablePosition in allSuitablePositions.OrderBy(item => item.Type)) 
                {
                    _validationResultBuilder.AddDeniedPositionsMessage(pricePosition, suitablePosition);
                }
            }
        }

        private bool AllLinkingObjectsAreSame(OrderPositionStateStorage first, OrderPositionStateStorage second)
        {
            return first.LinkingObjects.All(second.ContainsLinkingObjectLike)
                   && second.LinkingObjects.All(first.ContainsLinkingObjectLike);
        }
    }
}