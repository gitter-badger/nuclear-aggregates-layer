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
        private const decimal DefaultCategoryRate = 1;
        private readonly IFinder _finder;

        public PriceReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public PricePositionRateType GetPricePositionRateType(long pricePositionId)
        {
            return (PricePositionRateType)_finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.RateType).Single();
        }

        public decimal GetCategoryRateByFirm(long firmId)
        {
            var categoryQuery = _finder.Find(Specs.Find.ById<Firm>(firmId))
                                       .SelectMany(order => order.FirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                       .SelectMany(address => address.CategoryFirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                       .Select(addressCategory => addressCategory.Category)
                                       .Where(Specs.Find.ActiveAndNotDeleted<Category>());

            var orgUnitId = _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();

            return GetCategoryRateInternal(categoryQuery, orgUnitId);
        }

        // TODO {a.tukaev, 25.03.2014}: аргумент organizationUnitId лучше вычислять внутри этого метода получая от клиентского кода firmId, например также, как в методе GetCategoryRateByFirm
        public decimal GetCategoryRateByCategory(long categoryId, long organizationUnitId)
        {
            return GetCategoryRateInternal(_finder.Find(Specs.Find.ById<Category>(categoryId)), organizationUnitId);
        }

        public decimal GetPricePositionCost(long pricePositionId)
        {
            return _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId)).Select(x => x.Cost).Single();
        }


        private static decimal GetCategoryRateInternal(IQueryable<Category> categoryQuery, long organizationUnitId)
        {
            var categoryRate = categoryQuery.SelectMany(category => category.CategoryOrganizationUnits)
                                            .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                            .Where(categoryOrganizationUnit => categoryOrganizationUnit.OrganizationUnitId == organizationUnitId)
                                            .Select(categoryOrganizationUnit => (decimal?)(categoryOrganizationUnit.CategoryGroup != null
                                                                                               ? categoryOrganizationUnit.CategoryGroup.GroupRate
                                                                                               : DefaultCategoryRate))
                                            .Max();

            if (categoryRate == null)
            {
                throw new BusinessLogicException(BLResources.PricePositionCannotBeChoosedSinceThereIsNoFirmCategory);
            }

            return categoryRate.Value;
        }
    }
}