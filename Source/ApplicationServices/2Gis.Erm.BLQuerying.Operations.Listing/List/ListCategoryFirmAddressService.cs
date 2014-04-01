using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
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

            var data = query
                .Select(x => new
                {
                    CategoryFirmAddress = x,
                    CategoryOrganizationUnit = x.FirmAddress.Firm.OrganizationUnit.CategoryOrganizationUnits.FirstOrDefault(z => z.CategoryId == x.CategoryId),
                })
                .Select(x => new ListCategoryFirmAddressDto
                {
                    Id = x.CategoryFirmAddress.Id,
                    SortingPosition = x.CategoryFirmAddress.SortingPosition,
                    IsPrimary = x.CategoryFirmAddress.IsPrimary,
                    IsActive = x.CategoryFirmAddress.IsActive,
                    IsDeleted = x.CategoryFirmAddress.IsDeleted,
                    FirmAddressId = x.CategoryFirmAddress.FirmAddressId,
                    FirmId = x.CategoryFirmAddress.FirmAddress.FirmId,

                    CategoryId = x.CategoryFirmAddress.Category.Id,
                    Name = x.CategoryFirmAddress.Category.Name,
                    ParentId = x.CategoryFirmAddress.Category.ParentId,
                    ParentName = x.CategoryFirmAddress.Category.ParentCategory.Name,
                    Level = x.CategoryFirmAddress.Category.Level,
                    CategoryIsActive = x.CategoryFirmAddress.Category.IsActive,
                    CategoryIsDeleted = x.CategoryFirmAddress.Category.IsDeleted,

                    CategoryGroup = x.CategoryOrganizationUnit.CategoryGroup.CategoryGroupName,

                    CategoryOrganizationUnitIsActive = x.CategoryOrganizationUnit.IsActive,
                    CategoryOrganizationUnitIsDeleted = x.CategoryOrganizationUnit.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                // in memory
                .GroupBy(x => x.FirmId)
                .Select(x => new ListCategoryFirmAddressDto
                {
                    Id = x.First().Id,
                    SortingPosition = x.First().SortingPosition,
                    IsPrimary = x.First().IsPrimary,
                    IsActive = x.First().IsActive,
                    IsDeleted = x.First().IsDeleted,
                    FirmAddressId = x.First().FirmAddressId,
                    FirmId = x.Key,

                    CategoryId = x.First().CategoryId,
                    Name = x.First().Name,
                    ParentId = x.First().ParentId,
                    ParentName = x.First().ParentName,
                    Level = x.First().Level,
                    CategoryIsActive = x.First().CategoryIsActive,
                    CategoryIsDeleted = x.First().CategoryIsDeleted,

                    CategoryGroup = x.First().CategoryGroup ?? defaultCategoryRate,

                    CategoryOrganizationUnitIsActive = x.First().CategoryOrganizationUnitIsActive,
                    CategoryOrganizationUnitIsDeleted = x.First().CategoryOrganizationUnitIsDeleted,
                })
                .Skip(querySettings.SkipCount)
                .Take(querySettings.TakeCount)
                .ToArray();

            count = data.Length;
            return data;
        }
    }
}
