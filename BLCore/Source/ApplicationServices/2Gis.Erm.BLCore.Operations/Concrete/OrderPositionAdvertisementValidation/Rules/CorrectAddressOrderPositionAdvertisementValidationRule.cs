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
    // Проверка на то, что не используется скрытый или пустой адрес там, где его нельзя использовать
    public class CorrectAddressOrderPositionAdvertisementValidationRule : IAdvertisementValidationRule
    {
        private const OrderPositionAdvertisementValidationRule CurrentRule = OrderPositionAdvertisementValidationRule.CorrectAddress;
        private const long SponsoredLinkPositionCategoryId = 11;
        private const long AdvantageousPurchasePositionCategoryId = 14;
        private const long AddressCommentPositionCategoryId = 26;

        private readonly IFinder _finder;

        public CorrectAddressOrderPositionAdvertisementValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        public void Validate(AdvertisementDescriptor advertisement, ICollection<OrderPositionAdvertisementValidationError> errors)
        {
            if (!advertisement.FirmAddressId.HasValue)
            {
                return;
            }

            var validPositionCategoriesWithNotLocatedOnTheMapAddress = new[]
                {
                    SponsoredLinkPositionCategoryId,
                    AdvantageousPurchasePositionCategoryId,
                    AddressCommentPositionCategoryId
                };

            var positionCategoryInfo = _finder.Find(Specs.Find.ById<Position>(advertisement.PositionId))
                                              .Select(x =>
                                                      new
                                                          {
                                                              x.CategoryId,
                                                              x.Name
                                                          }).Single();

            var firmAddressInfo = _finder.Find(Specs.Find.ById<FirmAddress>(advertisement.FirmAddressId.Value))
                                         .Select(x => new
                                             {
                                                 IsHiden = !x.IsActive,
                                                 IsNotLocatedOnTheMap = !x.IsLocatedOnTheMap && x.Firm.OrganizationUnit.InfoRussiaLaunchDate != null,
                                             }).Single();

            if (firmAddressInfo.IsHiden ||
                (firmAddressInfo.IsNotLocatedOnTheMap && !validPositionCategoriesWithNotLocatedOnTheMapAddress.Contains(positionCategoryInfo.CategoryId)))
            {
                errors.Add(new OrderPositionAdvertisementValidationError
                    {
                        Rule = CurrentRule,
                        Advertisement = advertisement,
                        ErrorMessage =
                            string.Format(BLResources.InvalidAddressIsPickedForPosition,
                                          positionCategoryInfo.Name)
                    });
            }
        }
    }
}
