using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPrintFormTemplateService : ListEntityDtoServiceBase<PrintFormTemplate, ListPrintFormTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPrintFormTemplateService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<PrintFormTemplate>();

            var data = query
            .Select(x => new ListPrintFormTemplateDto
            {
                // filters
                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,

                Id = x.Id,
                FileId = x.FileId,
                FileName = x.File.FileName,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                TemplateCode = x.TemplateCode.ToStringLocalizedExpression(),
            })
            .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}