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
            long? organizationUnitId = null;

            if (querySettings.ParentEntityName == EntityName.Firm && querySettings.ParentEntityId != null)
            {
                organizationUnitId = _finder.Find(Specs.Find.ById<Firm>(querySettings.ParentEntityId.Value))
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
                    .Where(x => x.Category.CategoryOrganizationUnits.Where(y => y.OrganizationUnitId == organizationUnitId).Any(y => !y.IsDeleted))
                    .Filter(_filterHelper, firmFilterExpression)
                    .GroupBy(x => x.Category)
                    .Select(x => new ListCategoryFirmAddressDto
                        {
                            Id = 0,
                            CategoryGroup = organizationUnitId != null
                                ? x.Key.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == organizationUnitId.Value)
                                            .Select(unit => unit.CategoryGroup.CategoryGroupName).FirstOrDefault() ?? defaultCategoryRate
                                : string.Empty,
                            CategoryId = x.Key.Id,
                            Name = x.Key.Name,
                            ParentId = x.Key.ParentId,
                            ParentName = x.Key.ParentCategory.Name,
                            Level = x.Key.Level,
                            SortingPosition = x.Min(y => y.SortingPosition),
                            IsPrimary = x.Any(y => y.IsPrimary),
                            IsActive = x.Any(y => y.IsActive),
                            FirmId = querySettings.ParentEntityId.Value,
                            FirmAddressId = 0,
                        })
                    .QuerySettings(_filterHelper, querySettings, out count);
            }

            if (querySettings.ParentEntityName == EntityName.FirmAddress && querySettings.ParentEntityId != null)
            {
                organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(querySettings.ParentEntityId.Value))
                                                        .Select(address => address.Firm.OrganizationUnitId)
                                                        .Single();
            }

            return query
                .Where(x => !x.Category.IsDeleted && !x.FirmAddress.IsDeleted && !x.IsDeleted)
                .Where(x => x.Category.CategoryOrganizationUnits.Where(y => y.OrganizationUnitId == organizationUnitId).Any(y => !y.IsDeleted))
                .Select(x => new ListCategoryFirmAddressDto
                    {
                        Id = x.Id,
                        CategoryGroup = organizationUnitId.HasValue
                            ? x.Category.CategoryOrganizationUnits.Where(unit => unit.OrganizationUnitId == organizationUnitId.Value)
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
