using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCategoryFirmAddressService : ListEntityDtoServiceBase<CategoryFirmAddress, ListCategoryFirmAddressDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListCategoryFirmAddressService(
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListCategoryFirmAddressDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<CategoryFirmAddress>();

            var defaultCategoryRate = string.Format(_userContext.Profile.UserLocaleInfo.UserCultureInfo, "{0:p0}", 1);
            long? categoryRateOrganizationUnitId = null;

            if (querySettings.ParentEntityName == EntityName.Firm && querySettings.ParentEntityId != null)
            {
                categoryRateOrganizationUnitId = _finder.Find(Specs.Find.ById<Firm>(querySettings.ParentEntityId.Value))
                                                        .Select(firm => firm.OrganizationUnitId)
                                                        .Single();

                var firmFilterExpression = querySettings.CreateForExtendedProperty<CategoryFirmAddress, bool>("IsActive",
                isActive =>
                {
                        if (isActive)
                        {
                            return x => !x.Category.IsDeleted &&
                                                x.Category.IsActive &&
                                                x.IsActive && !x.IsDeleted && x.FirmAddress.IsActive &&
                                                !x.FirmAddress.IsDeleted && !x.FirmAddress.ClosedForAscertainment &&
                                                x.FirmAddress.FirmId == querySettings.ParentEntityId.Value;
                }

                        return x => x.Category.IsActive &&
                                                !x.IsActive &&
                                                x.FirmAddress.IsActive &&
                                                x.FirmAddress.FirmId == querySettings.ParentEntityId.Value;
                    });

                // заполняем только то что связано с рубрикой и делаем Distinct
                return query
                    .Where(x => !x.Category.IsDeleted && !x.FirmAddress.IsDeleted && !x.IsDeleted)
                    .Filter(_filterHelper, firmFilterExpression)
                    .Select(x => new ListCategoryFirmAddressDto
                        {
                            CategoryGroup = categoryRateOrganizationUnitId.HasValue
                                ? x.Category.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == categoryRateOrganizationUnitId.Value)
                                            .Select(unit => unit.CategoryGroup.CategoryGroupName).FirstOrDefault() ?? defaultCategoryRate
                                : string.Empty,
                        CategoryId = x.Category.Id,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        ParentName = x.Category.ParentCategory.Name,
                        Level = x.Category.Level,
                        SortingPosition = 0,
                        IsPrimary = true,
                        IsActive = true,
                        FirmId = querySettings.ParentEntityId.Value,
                        FirmAddressId = 0,
                        Id = 0,
                        })
                    .Distinct()
                    .QuerySettings(_filterHelper, querySettings, out count);
            }

            if (querySettings.ParentEntityName == EntityName.FirmAddress && querySettings.ParentEntityId != null)
            {
                categoryRateOrganizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(querySettings.ParentEntityId.Value))
                                                        .Select(address => address.Firm.OrganizationUnitId)
                                                        .Single();
            }

            return query
                .Where(x => !x.Category.IsDeleted && !x.FirmAddress.IsDeleted && !x.IsDeleted)
                .Select(x => new ListCategoryFirmAddressDto
                    {
                        Id = x.Id,
                        CategoryGroup = categoryRateOrganizationUnitId.HasValue
                            ? x.Category.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == categoryRateOrganizationUnitId.Value)
                                        .Select(unit => unit.CategoryGroup.CategoryGroupName).FirstOrDefault() ?? defaultCategoryRate
                            : string.Empty,
                        FirmAddressId = x.FirmAddressId,
                        SortingPosition = x.SortingPosition,

                        CategoryId = x.Category.Id,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        ParentName = x.Category.ParentCategory != null ? x.Category.ParentCategory.Name : null,
                        Level = x.Category.Level,

                        IsPrimary = x.IsPrimary,
                        IsActive = x.IsActive,
                        FirmId = x.FirmAddress.FirmId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}
