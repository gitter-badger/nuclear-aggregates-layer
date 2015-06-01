using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositionAdvertisementValidation.Rules
{
    // Проверка на то, что выбраны РМ везде, где нужно
    public class RequiredAdvertisementOrderPositionAdvertisementValidationRule : IAdvertisementValidationRule
    {
        private const OrderPositionAdvertisementValidationRule CurrentRule = OrderPositionAdvertisementValidationRule.RequiredAdvertisement;
        private readonly IFinder _finder;

        public RequiredAdvertisementOrderPositionAdvertisementValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        public void Validate(AdvertisementDescriptor advertisement, ICollection<OrderPositionAdvertisementValidationError> errors)
        {
            if (advertisement.AdvertisementId.HasValue)
            {
                return;
            }

            var positionInfo = _finder.FindObsolete(Specs.Find.ById<Position>(advertisement.PositionId)).Select(x => new
                {
                    PositionName = x.Name,
                    x.AdvertisementTemplate
                }).Single();

            if (positionInfo.AdvertisementTemplate == null || !positionInfo.AdvertisementTemplate.IsAdvertisementRequired)
            {
                return;
            }

            errors.Add(new OrderPositionAdvertisementValidationError
                {
                    Rule = CurrentRule,
                    Advertisement = advertisement,
                    ErrorMessage = string.Format(BLResources.ThereIsNoAdvertisementsForSomeLinkingObjects, positionInfo.PositionName)
                });
        }
    }
}
