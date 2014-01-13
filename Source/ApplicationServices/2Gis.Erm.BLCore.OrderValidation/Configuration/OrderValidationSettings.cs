using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;

namespace DoubleGis.Erm.BLCore.OrderValidation.Configuration
{
    public sealed class OrderValidationSettings : IOrderValidationSettings
    {
        public IDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalDeniedPositions(IEnumerable<long> requiredPositionIds)
        {
            var deniedPositions = requiredPositionIds.ToDictionary(id => id,
                                                                   id =>
                                                                   LoadGlobalPrincipalConfiguration(id,
                                                                                                    position =>
                                                                                                    position.DeniedPositions.Cast<AssociatedDeniedPosition>())
                                                                       .ToArray());

            var symetricDeniedRules =
                new Dictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>>();

            foreach (var deniedPosition in deniedPositions)
            {
                var deniedPositionSymetricDeniedRules = deniedPositions.SelectMany(x => x.Value.Select(y => new
                    {
                        PositionId = y.PositionId,
                        Key = x.Key,
                        BindingCheckMode = y.BindingCheckMode
                    }))
                                                                       .Where(x => x.PositionId == deniedPosition.Key)
                                                                       .Select(
                                                                           x =>
                                                                           new PricePositionDto.RelatedItemDto
                                                                               {
                                                                                   BindingCheckMode = x.BindingCheckMode,
                                                                                   PositionId = x.Key
                                                                               }).ToArray();

                symetricDeniedRules.Add(deniedPosition.Key, deniedPositionSymetricDeniedRules);
            }

            foreach (var symetricDeniedRule in symetricDeniedRules)
            {
                if (deniedPositions.ContainsKey(symetricDeniedRule.Key))
                {
                    deniedPositions[symetricDeniedRule.Key] = deniedPositions[symetricDeniedRule.Key].Union(symetricDeniedRule.Value).ToArray();
                }
                else
                {
                    deniedPositions.Add(symetricDeniedRule.Key, symetricDeniedRule.Value.ToArray());
                }
            }

            return deniedPositions.ToDictionary(x => x.Key, x => x.Value.AsEnumerable());
        }

        public IDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalAssociatedPositions(IEnumerable<long> requiredPositionIds)
        {
            return requiredPositionIds.ToDictionary(id => id,
                                            id => LoadGlobalPrincipalConfiguration(id, position => position.MasterPositions.Cast<AssociatedDeniedPosition>()));
        }

        private IEnumerable<PricePositionDto.RelatedItemDto> LoadGlobalPrincipalConfiguration(long positionId,
                                                                                              Func<AssociatedDeniedPosition, IEnumerable<AssociatedDeniedPosition>> selector)
        {
            var globalSettings = OrderValidationSettingsConfigurationSection.Instance;
            if (globalSettings == null)
            {
                return new PricePositionDto.RelatedItemDto[0];
            }

            return globalSettings.AssociatedDeniedPositions.Cast<AssociatedDeniedPosition>()
                .Where(position => position.Id == positionId)
                .SelectMany(selector)
                .Select(position => new PricePositionDto.RelatedItemDto { BindingCheckMode = position.BindingType, PositionId = position.Id })
                .ToArray();
        }

    }
}
