using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositionAdvertisementValidation.Rules
{
    // Проверка на то, что шаблон выбранного РМ совпадает с шаблоном определенным в номенклатурной позиции
    public class AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRule : IAdvertisementValidationRule
    {
        private const OrderPositionAdvertisementValidationRule CurrentRule = OrderPositionAdvertisementValidationRule.AdvertisementTemplateMatchesPositionTemplate;
        private readonly IFinder _finder;

        public AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        public void Validate(AdvertisementDescriptor advertisement, ICollection<OrderPositionAdvertisementValidationError> errors)
        {
            if (!advertisement.AdvertisementId.HasValue)
            {
                return;
            }

            var advertisementTemplateId = _finder.Find(Specs.Find.ById<Advertisement>(advertisement.AdvertisementId.Value))
                                                 .Select(x => x.AdvertisementTemplateId).Single();

            var positionTemplateInfo = _finder.Find(Specs.Find.ById<Position>(advertisement.PositionId))
                                              .Select(x =>
                                                      new
                                                          {
                                                              x.AdvertisementTemplateId,
                                                              x.Name
                                                          }).Single();

            if (advertisementTemplateId != positionTemplateInfo.AdvertisementTemplateId)
            {
                errors.Add(new OrderPositionAdvertisementValidationError
                    {
                        Rule = CurrentRule,
                        Advertisement = advertisement,
                        ErrorMessage = string.Format(BLResources.AdvertisementTemplateDifferentFromPositionAdvertisementTemplate,
                                                     positionTemplateInfo.Name)
                    });
            }
        }
    }
}
