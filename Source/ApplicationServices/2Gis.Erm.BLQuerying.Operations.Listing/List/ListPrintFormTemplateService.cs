using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPrintFormTemplateService : ListEntityDtoServiceBase<PrintFormTemplate, ListPrintFormTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListPrintFormTemplateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPrintFormTemplateDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<PrintFormTemplate>();

            var data = query
            .DefaultFilter(_filterHelper, querySettings)
            .Select(x => new
            {
                // filters
                x.BranchOfficeOrganizationUnitId,
                x.IsActive,
                x.IsDeleted,

                x.Id,
                x.FileId,
                TemplateCode = (TemplateCode)x.TemplateCode,
                x.File.FileName,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x => new ListPrintFormTemplateDto
            {
                Id = x.Id,
                FileId = x.FileId,
                TemplateCode = x.TemplateCode.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                FileName = x.FileName,
                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnitName
            });

            return data;
        }
    }
}