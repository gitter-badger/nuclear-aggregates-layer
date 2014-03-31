using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public class SupportedCategoriesChecker : ISupportedCategoriesChecker
    {
        private readonly ICategoryService _categoryService;

        public SupportedCategoriesChecker(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public bool IsSupported(PricePositionRateType rateType, long categoryId, long destOrganizationUnitId)
        {
            if (rateType != PricePositionRateType.BoundCategory)
            {
                return true;
            }

            if (!NewSalesModelRestrictions.SupportedCategoryIds.Contains(categoryId) ||
                !NewSalesModelRestrictions.SupportedOrganizationUnitIds.Contains(destOrganizationUnitId))
            {
                return false;
            }

            return _categoryService.IsCategoryLinkedWithOrgUnit(categoryId, destOrganizationUnitId);
        }

        public void Check(PricePositionRateType rateType, long categoryId, long destOrganizationUnitId)
        {
            if (!IsSupported(rateType, categoryId, destOrganizationUnitId))
            {
                var categoryName = _categoryService.GetCategoryName(categoryId);
                throw new NotificationException(string.Format(BLResources.CategoryIsNotSupportedForSpecifiedRateTypeAndOrgUnit,
                                                              rateType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                                              categoryName));
            }
        }
    }
}