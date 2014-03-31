using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
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

            var query2 = query.Select(x => new QueryMiddleDto
            {
                FirmId = x.FirmAddress.FirmId,
                CategoryFirmAddress = x,
            });

            if (querySettings.ParentEntityName == EntityName.Firm)
            {
                query2 = query2
                .GroupBy(x => x.FirmId)
                .Select(x => new QueryMiddleDto
                {
                    FirmId = x.Key,
                    CategoryFirmAddress = x.Select(y => y.CategoryFirmAddress).FirstOrDefault(),
                });
            }

            var defaultCategoryRate = string.Format(_userContext.Profile.UserLocaleInfo.UserCultureInfo, "{0:p0}", 1);

            return query2
                .Select(x => new
                {
                    x.FirmId,
                    x.CategoryFirmAddress,
                    CategoryOrganizationUnit = x.CategoryFirmAddress.FirmAddress.Firm.OrganizationUnit.CategoryOrganizationUnits.FirstOrDefault(y => y.CategoryId == x.CategoryFirmAddress.CategoryId),
                })
                .Select(x => new ListCategoryFirmAddressDto
                {
                    Id = x.CategoryFirmAddress.Id,
                    SortingPosition = x.CategoryFirmAddress.SortingPosition,
                    IsPrimary = x.CategoryFirmAddress.IsPrimary,
                    IsActive = x.CategoryFirmAddress.IsActive,
                    IsDeleted = x.CategoryFirmAddress.IsDeleted,
                    FirmAddressId = x.CategoryFirmAddress.FirmAddressId,
                    FirmId = x.FirmId,

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
                .Select(x =>
                {
                    x.CategoryGroup = x.CategoryGroup ?? defaultCategoryRate;
                    return x;
                });
        }

        private sealed class QueryMiddleDto
        {
            public long FirmId { get; set; }
            public CategoryFirmAddress CategoryFirmAddress { get; set; }
        }
    }
}
