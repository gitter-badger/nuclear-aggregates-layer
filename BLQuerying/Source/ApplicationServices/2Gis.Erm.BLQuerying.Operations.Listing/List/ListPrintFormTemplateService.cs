using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPrintFormTemplateService : ListEntityDtoServiceBase<PrintFormTemplate, ListPrintFormTemplateDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListPrintFormTemplateService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<PrintFormTemplate>();

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