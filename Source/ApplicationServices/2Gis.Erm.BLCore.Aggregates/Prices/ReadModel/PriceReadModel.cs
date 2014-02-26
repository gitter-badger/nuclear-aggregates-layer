using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel
{
    public class PriceReadModel : IPriceReadModel
    {
        private readonly IFinder _finder;

        public PriceReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public PricePositionRateType GetPricePositionRateType(long pricePositionId)
        {
            return (PricePositionRateType)_finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.RateType).Single();
        }

        public decimal GetCategoryRate(long pricePositionId, long firmId, long? categoryId)
        {
            const decimal DefaultRate = 1;
            var rateType = (PricePositionRateType)_finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.RateType).Single();

            if (rateType == PricePositionRateType.None)
            {
                return DefaultRate;
            }

            var orgUnitId = _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();
            var categoryRateQuery = _finder.Find(Specs.Find.ById<Firm>(firmId))
                                           .SelectMany(order => order.FirmAddresses)
                                           .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                           .SelectMany(address => address.CategoryFirmAddresses)
                                           .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                           .Select(addressCategory => addressCategory.Category)
                                           .Where(Specs.Find.ActiveAndNotDeleted<Category>());

            if (rateType == PricePositionRateType.BoundCategory)
            {
                if (categoryId == null)
                {
                    throw new NotificationException(
                        BLResources.CategoryShouldBeSpecifiedForTheBoundCategoryRateType);
                }

                categoryRateQuery = categoryRateQuery.Where((Specs.Find.ById<Category>(categoryId.Value)));
            }

            var categoryRate = categoryRateQuery.SelectMany(category => category.CategoryOrganizationUnits)
                                                .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                                .Where(categoryOrganizationUnit => categoryOrganizationUnit.OrganizationUnitId == orgUnitId)
                                                .Select(categoryOrganizationUnit => (decimal?)(categoryOrganizationUnit.CategoryGroup != null
                                                                                                   ? categoryOrganizationUnit.CategoryGroup.GroupRate
                                                                                                   : DefaultRate))
                                                .Max();

            if (categoryRate == null)
            {
                throw new BusinessLogicException(BLResources.PricePositionCannotBeChoosedSinceThereIsNoFirmCategory);
            }

            return categoryRate.Value;
        }
    }
}