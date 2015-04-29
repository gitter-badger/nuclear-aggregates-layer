using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCategoryOrganizationUnitService : ListEntityDtoServiceBase<CategoryOrganizationUnit, ListCategoryOrganizationUnitDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListCategoryOrganizationUnitService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<CategoryOrganizationUnit>();

            return query
                .Select(x => new ListCategoryOrganizationUnitDto
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        OrganizationUnitId = x.OrganizationUnitId,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        IsDeleted = x.IsDeleted,
                        CategoryIsDeleted = x.Category.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}