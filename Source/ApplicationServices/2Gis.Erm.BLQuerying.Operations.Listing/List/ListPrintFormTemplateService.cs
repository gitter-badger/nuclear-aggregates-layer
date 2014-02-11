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
    public class ListPrintFormTemplateService : ListEntityDtoServiceBase<PrintFormTemplate, ListPrintFormTemplateDto>
    {
        public ListPrintFormTemplateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListPrintFormTemplateDto> GetListData(IQueryable<PrintFormTemplate> query, QuerySettings querySettings, out int count)
        {
            var data = query
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
            .ApplyQuerySettings(querySettings, out count)
            .AsEnumerable()
            .Select(x => new ListPrintFormTemplateDto
            {
                Id = x.Id,
                FileId = x.FileId,
                TemplateCode = x.TemplateCode.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                FileName = x.FileName,
                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnitName
            });

            return data;
        }
    }
}