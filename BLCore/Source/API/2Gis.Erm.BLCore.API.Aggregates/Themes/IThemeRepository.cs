using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes.DTO;
using DoubleGis.Erm.Platform.API.Core;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Themes
{
    public interface IThemeRepository : IAggregateRootRepository<Theme>,
                                        IDeleteAggregateRepository<Theme>,
                                        IDeleteAggregateRepository<ThemeTemplate>,
                                        IDeleteAggregateRepository<ThemeCategory>,
                                        IDeleteAggregateRepository<ThemeOrganizationUnit>,
                                        IDownloadFileAggregateRepository<Theme>,
                                        IDownloadFileAggregateRepository<ThemeTemplate>,
                                        IUploadFileAggregateRepository<Theme>,
                                        IUploadFileAggregateRepository<ThemeTemplate>
    {
        Theme FindTheme(long themeId);
        ThemeTemplate FindThemeTemplateByThemeId(long themeTemplateId);
        ThemeTemplate FindThemeTemplate(long themeTemplateId);
        ThemeTemplate FindThemeTemplateByThemplateCode(long templateCode);
        File GetThemeTemplateFile(long themeTemplateId);
        File GetThemeFile(long themeId);

        void CreateOrUpdate(ThemeTemplate template);
        void CreateOrUpdate(Theme theme);

        bool CanThemeBeDefault(long themeId);
        bool IsThemeUsedInOrders(long themeId);
        bool IsTemplateUsedInThemes(long templateId);
        bool IsThemeAppendedToOrganizationUnit(long themeId, long organizationUnitId);
        int CountThemeCategories(long themeId);
        bool IsThemeLimitReachedInOrganizationUnit(Theme theme, long organizationUnitId);
        string GetBase64EncodedFile(long fileId);
        string GetOrganizationUnitName(long organizationUnitId);

        IEnumerable<ThemeTemplateUsageDto> GetThemeUsage(long organizationUnit, TimePeriod period);
        int CountThemeOrganizationUnits(long themeId);

        ThemeOrganizationUnit AppendThemeToOrganizationUnit(long themeId, long organizationUnitId);
        ThemeCategory AppendThemeToCategory(long themeId, long categoryId);
    }
}