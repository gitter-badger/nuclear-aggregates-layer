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
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPrintFormTemplateDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<PrintFormTemplate>();

            var data = query
            .Select(x => new ListPrintFormTemplateDto
            {
                // filters
                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,

                Id = x.Id,
                FileId = x.FileId,
                TemplateCodeEnum = (TemplateCode)x.TemplateCode,
                FileName = x.File.FileName,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                TemplateCode = null,
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x =>
            {
                x.TemplateCode = x.TemplateCodeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                return x;
            });

            return data;
        }
    }
}