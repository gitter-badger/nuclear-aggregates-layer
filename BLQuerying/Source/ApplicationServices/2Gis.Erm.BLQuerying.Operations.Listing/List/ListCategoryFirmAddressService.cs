using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

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

        protected override IRemoteCollection List(QuerySettings querySettings)
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
                    FirmId = x.CategoryFirmAddress.FirmAddress.FirmId,

                    CategoryId = x.CategoryFirmAddress.Category.Id,
                    Name = x.CategoryFirmAddress.Category.Name,
                    ParentId = x.CategoryFirmAddress.Category.ParentId,
                    ParentName = x.CategoryFirmAddress.Category.ParentCategory.Name,
                    Level = x.CategoryFirmAddress.Category.Level,
                    CategoryIsActive = x.CategoryFirmAddress.Category.IsActive,
                    CategoryIsDeleted = x.CategoryFirmAddress.Category.IsDeleted,

                    FirmAddressId = x.CategoryFirmAddress.FirmAddressId,
                    FirmAddressIsActive = x.CategoryFirmAddress.FirmAddress.IsActive,
                    FirmAddressIsDeleted = x.CategoryFirmAddress.FirmAddress.IsDeleted,

                    CategoryGroup = x.CategoryOrganizationUnit.CategoryGroup.Name ?? defaultCategoryRate,
                    CategoryOrganizationUnitIsActive = x.CategoryOrganizationUnit != null ? x.CategoryOrganizationUnit.IsActive : true,
                    CategoryOrganizationUnitIsDeleted = x.CategoryOrganizationUnit != null ? x.CategoryOrganizationUnit.IsDeleted : false,
                });

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Firm()) && querySettings.ParentEntityId.HasValue)
            {
                long firmId = querySettings.ParentEntityId.Value;
                data = data
                    .Where(x => x.FirmId == firmId)

                    // не рассматриваем неактивные адреса фирм вообще
                    .Where(x => x.FirmAddressIsActive && !x.FirmAddressIsDeleted)
                    .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Name,
                    x.ParentId,
                    x.ParentName,
                    x.Level,
                    x.CategoryIsActive,
                    x.CategoryIsDeleted,
                    x.FirmId,
                })
                .Select(x => new ListCategoryFirmAddressDto
                {
                    Id = 0,
                    SortingPosition = x.Max(y => y.SortingPosition),
                    IsPrimary = x.Any(y => y.IsPrimary),
                    IsActive = x.Any(y => y.IsActive),
                    IsDeleted = x.All(y => y.IsDeleted),
                    FirmId = x.Key.FirmId,

                    CategoryId = x.Key.CategoryId,
                    Name = x.Key.Name,
                    ParentId = x.Key.ParentId,
                    ParentName = x.Key.ParentName,
                    Level = x.Key.Level,
                    CategoryIsActive = x.Key.CategoryIsActive,
                    CategoryIsDeleted = x.Key.CategoryIsDeleted,

                    FirmAddressId = 0,
                    FirmAddressIsActive = true,
                    FirmAddressIsDeleted = false,

                    CategoryGroup = x.Select(y => y.CategoryGroup).FirstOrDefault() ?? defaultCategoryRate,
                    CategoryOrganizationUnitIsActive = x.Any(y => y.CategoryOrganizationUnitIsActive),
                    CategoryOrganizationUnitIsDeleted = x.All(y => y.CategoryOrganizationUnitIsDeleted),
                });
            }

            return data
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}
