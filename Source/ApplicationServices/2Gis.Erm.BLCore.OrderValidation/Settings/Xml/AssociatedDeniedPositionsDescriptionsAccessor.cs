using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings.Xml
{
    public static class AssociatedDeniedPositionsDescriptionsAccessor
    {
        private const string OrderValidationSettingsSectionName = "orderValidationSettings";

        public static IEnumerable<PricePositionDescription> GetPricePositionDescriptions()
        {
            return GetPricePositionDescriptions(ConfigurationManager.GetSection);
        }

        public static IEnumerable<PricePositionDescription> GetPricePositionDescriptions(Configuration configuration)
        {
            return GetPricePositionDescriptions(configuration.GetSection);
        }

        private static IEnumerable<PricePositionDescription> GetPricePositionDescriptions(
            Func<string, object> orderValidationSettingsSectionLoader)
        {
            var configSection = (OrderValidationSettingsConfigurationSection)orderValidationSettingsSectionLoader(OrderValidationSettingsSectionName);
            return configSection == null
                ? Enumerable.Empty<PricePositionDescription>()
                : configSection.AssociatedDeniedPositions
                                    .Cast<AssociatedDeniedPosition>()
                                    .Aggregate(new Dictionary<long, PricePositionDescription>(),
                                               (dictionary, position) =>
                                                   {
                                                       dictionary.Add(position.Id,
                                                                      new PricePositionDescription
                                                                          {
                                                                              PositionId = position.Id,
                                                                              MasterPositions = ExtractPositionItems(position.MasterPositions),
                                                                              DeniedPositions = ExtractPositionItems(position.DeniedPositions)
                                                                          });
                                                       return dictionary;
                                                   })
                                    .Values;
        }

        private static PricePositionDto.RelatedItemDto[] ExtractPositionItems(AssociatedDeniedPositions associatedDeniedPosition)
        {
            return associatedDeniedPosition.Cast<AssociatedDeniedPosition>()
                                            .Select(p => 
                                                new PricePositionDto.RelatedItemDto
                                                        {
                                                            PositionId = p.Id, 
                                                            BindingCheckMode = p.BindingType
                                                        })
                                            .ToArray();
        }
    }
}