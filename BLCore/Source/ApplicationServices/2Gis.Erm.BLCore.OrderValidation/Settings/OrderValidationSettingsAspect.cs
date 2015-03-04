using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings
{
    public sealed class OrderValidationSettingsAspect : ISettingsAspect, IOrderValidationSettings
    {
        private readonly Dictionary<long, PricePositionDto.RelatedItemDto[]> _masterPositionsMap;
        private readonly Dictionary<long, Dictionary<long, PricePositionDto.RelatedItemDto>> _deniedPositionWithSymmetricDeniedRulesMap;

        public OrderValidationSettingsAspect(IEnumerable<PricePositionDescription> positionDescriptions)
        {
            _masterPositionsMap = new Dictionary<long, PricePositionDto.RelatedItemDto[]>();
            _deniedPositionWithSymmetricDeniedRulesMap = new Dictionary<long, Dictionary<long, PricePositionDto.RelatedItemDto>>();

            foreach (var positionDescription in positionDescriptions)
            {
                if (positionDescription.MasterPositions != null && positionDescription.MasterPositions.Any())
                {
                    _masterPositionsMap.Add(positionDescription.PositionId, positionDescription.MasterPositions);
                }

                if (positionDescription.DeniedPositions != null && positionDescription.DeniedPositions.Any())
                {
                    Dictionary<long, PricePositionDto.RelatedItemDto> deniedMap;
                    if (!_deniedPositionWithSymmetricDeniedRulesMap.TryGetValue(positionDescription.PositionId, out deniedMap))
                    {
                        deniedMap = new Dictionary<long, PricePositionDto.RelatedItemDto>();
                        _deniedPositionWithSymmetricDeniedRulesMap.Add(positionDescription.PositionId, deniedMap);
                    }

                    foreach (var deniedPosition in positionDescription.DeniedPositions)
                    {
                        TryAddDeniedPosition(deniedMap, deniedPosition.PositionId, deniedPosition);

                        Dictionary<long, PricePositionDto.RelatedItemDto> symetricDeniedMap;
                        if (!_deniedPositionWithSymmetricDeniedRulesMap.TryGetValue(deniedPosition.PositionId, out symetricDeniedMap))
                        {
                            symetricDeniedMap = new Dictionary<long, PricePositionDto.RelatedItemDto>();
                            _deniedPositionWithSymmetricDeniedRulesMap.Add(deniedPosition.PositionId, symetricDeniedMap);
                        }

                        TryAddDeniedPosition(symetricDeniedMap,
                                             positionDescription.PositionId,
                                             new PricePositionDto.RelatedItemDto
                                                 {
                                                     PositionId = positionDescription.PositionId,
                                                     BindingCheckMode = deniedPosition.BindingCheckMode
                                                 });
                    }
                }
            }
        }

        public IReadOnlyDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalDeniedPositions(IEnumerable<long> requiredPositionIds)
        {
            var positions = new Dictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>>();
            foreach (var positionId in requiredPositionIds)
            {
                Dictionary<long, PricePositionDto.RelatedItemDto> deniedPositionsMap;
                positions.Add(
                    positionId, 
                    !_deniedPositionWithSymmetricDeniedRulesMap.TryGetValue(positionId, out deniedPositionsMap) ? Enumerable.Empty<PricePositionDto.RelatedItemDto>() : deniedPositionsMap.Values);
            }

            return positions;
        }

        public IReadOnlyDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalAssociatedPositions(IEnumerable<long> requiredPositionIds)
        {
            var positions = new Dictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>>();
            foreach (var positionId in requiredPositionIds)
            {
                PricePositionDto.RelatedItemDto[] masterPositions;
                positions.Add(
                    positionId,
                    !_masterPositionsMap.TryGetValue(positionId, out masterPositions) ? Enumerable.Empty<PricePositionDto.RelatedItemDto>() : masterPositions);
            }

            return positions;
        }

        private bool TryAddDeniedPosition(IDictionary<long, PricePositionDto.RelatedItemDto> targetDeniedPositionsMap,
                                          long candidateKey,
                                          PricePositionDto.RelatedItemDto candidateInstance)
        {
            PricePositionDto.RelatedItemDto existingInstance;
            if (targetDeniedPositionsMap.TryGetValue(candidateKey, out existingInstance))
            {
                if (existingInstance.BindingCheckMode != candidateInstance.BindingCheckMode)
                {
                    throw new InvalidOperationException(
                                string.Format("Trying to reuse symetric denied position entry, but existing BindingCheckMode={0} is not equal reusing BindingCheckMode={1}." +
                                              " Position id: {2}. Symetric denied position id: {3}.",
                                              existingInstance.BindingCheckMode,
                                              candidateInstance.BindingCheckMode,
                                              candidateKey,
                                              candidateInstance.PositionId));
                }

                return false;
            }

            targetDeniedPositionsMap.Add(candidateKey, candidateInstance);
            return true;
        }
    }
}
