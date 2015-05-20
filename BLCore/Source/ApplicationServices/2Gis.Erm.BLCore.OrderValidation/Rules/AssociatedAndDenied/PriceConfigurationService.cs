using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    public class PriceConfigurationService : IPriceConfigurationService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IOrderValidationSettings _orderValidationSettings;
        private Dictionary<KeyValuePair<long, long>, PriceConfigurationStorage> _pricePositionsRules;

        public PriceConfigurationService(IPriceRepository priceRepository, IOrderValidationSettings orderValidationSettings)
        {
            _priceRepository = priceRepository;
            _orderValidationSettings = orderValidationSettings;
        }

        public void LoadConfiguration(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds, IList<OrderValidationMessage> messages)
        {
            var pricePositionsRules = new Dictionary<KeyValuePair<long, long>, PriceConfigurationStorage>();
            var pricePositions = _priceRepository.GetPricePositions(requiredPriceIds, requiredPositionIds);
            var globalPrincipalPositions = _orderValidationSettings.GetGlobalAssociatedPositions(requiredPositionIds);
            var globalDeniedPositions = _orderValidationSettings.GetGlobalDeniedPositions(requiredPositionIds);

            // Делаем декартово произведение requiredPriceIds и requiredPositionIds
            // и заполняем пустыми списками
            var fakePricePositionDtos = requiredPriceIds.SelectMany(x => requiredPositionIds, (x, y) => new
                {
                    PricePositionDto = new PricePositionDto
                        {
                            DeniedPositions = new List<PricePositionDto.RelatedItemDto>(),
                            PriceId = x,
                            PositionId = y,
                            Groups = new List<IEnumerable<PricePositionDto.RelatedItemDto>>(),
                        }
                });

            // Недостающие комбинации берем из пустышек, которые получили выше
            var allPricePositions = pricePositions.Union(from fakePricePositionDto in fakePricePositionDtos
                                     where
                                         !pricePositions.Any(
                                             x =>
                                             x.PriceId == fakePricePositionDto.PricePositionDto.PriceId &&
                                             x.PositionId == fakePricePositionDto.PricePositionDto.PositionId)
                                     select fakePricePositionDto.PricePositionDto).ToList();

            foreach (var pricePosition in allPricePositions)
            {
                var principalPositions = new List<PricePositionDto.RelatedItemDto>();
                foreach (var group in pricePosition.Groups)
                {
                    if (principalPositions.Count > 0)
                    {
                        var pricePositionDescriptionTemplate = string.Format("<{0}:{1}:{2}>", EntityType.Instance.PricePosition().Description, pricePosition.PositionName, pricePosition.Id);
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Warning,
                                MessageText = string.Format(BLResources.InPricePositionOf_Price_ContaiedMoreThanOneAssociatedPositions, pricePositionDescriptionTemplate)
                            });
                        break;
                    }

                    principalPositions.AddRange(group);
                }


                var denied = MergeRelatedPositions(pricePosition.PositionId, globalDeniedPositions, pricePosition.DeniedPositions);
                var principal = MergeRelatedPositions(pricePosition.PositionId, globalPrincipalPositions, principalPositions);
                var key = new KeyValuePair<long, long>(pricePosition.PriceId, pricePosition.PositionId);
                pricePositionsRules[key] = new PriceConfigurationStorage(principal, denied);
            }

            _pricePositionsRules = pricePositionsRules;
        }

        public PriceConfigurationStorage GetPriceConfigurationStorage(long priceId, long positionId)
        {
            PriceConfigurationStorage storage;
            var key = new KeyValuePair<long, long>(priceId, positionId);
            return _pricePositionsRules.TryGetValue(key, out storage) ? storage : PriceConfigurationStorage.Empty;
        }

        private IEnumerable<PricePositionDto.RelatedItemDto> MergeRelatedPositions(long positionId,
                                                                   IReadOnlyDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> globalPositions,
                                                                   IEnumerable<PricePositionDto.RelatedItemDto> pricePostions)
        {
            IEnumerable<PricePositionDto.RelatedItemDto> positions;
            if (globalPositions.TryGetValue(positionId, out positions) && positions != null && positions.Any())
            {
                return pricePostions.Union(positions);
            }

            return pricePostions;
        }
    }
}