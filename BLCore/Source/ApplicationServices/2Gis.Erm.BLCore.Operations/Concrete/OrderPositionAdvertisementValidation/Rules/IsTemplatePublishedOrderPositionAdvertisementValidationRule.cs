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
    // Проверка на то, что используется неопубликованный шаблон
    // TODO {d.ivanov, 16.12.2013}: 2+ BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\OrderPositionAdvertisementValidation\Rules
    public class IsTemplatePublishedOrderPositionAdvertisementValidationRule : IAdvertisementValidationRule
    {
        private const OrderPositionAdvertisementValidationRule CurrentRule = OrderPositionAdvertisementValidationRule.IsTemplatePublished;
        private readonly IFinder _finder;

        public IsTemplatePublishedOrderPositionAdvertisementValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        public void Validate(AdvertisementDescriptor advertisement, ICollection<OrderPositionAdvertisementValidationError> errors)
        {
            if (!advertisement.AdvertisementId.HasValue ||
                !_finder.Find(Specs.Find.ById<Advertisement>(advertisement.AdvertisementId.Value))
                        .Any(x => !x.AdvertisementTemplate.IsPublished))
            {
                return;
            }

            var positionName = _finder.Find(Specs.Find.ById<Position>(advertisement.PositionId)).Single().Name;

            errors.Add(new OrderPositionAdvertisementValidationError
                {
                    Rule = CurrentRule,
                    Advertisement = advertisement,
                    ErrorMessage = string.Format(BLResources.AdvertisementWithUnpublishedTemplateIsChosen, positionName)
                });
        }
    }
}
