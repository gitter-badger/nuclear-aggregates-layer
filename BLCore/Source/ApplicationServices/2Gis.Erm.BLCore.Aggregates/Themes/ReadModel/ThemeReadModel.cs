using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Themes.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Themes.ReadModel
{
    public sealed class ThemeReadModel : IThemeReadModel
    {
        private readonly IFinder _finder;

        public ThemeReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<LinkingObjectsSchemaDto.ThemeDto> FindThemesCanBeUsedWithOrder(long destOrganizationUnitId, DateTime beginDistributionDate, DateTime endDistributionDate)
        {
            return _finder.FindObsolete(Specs.Find.ById<OrganizationUnit>(destOrganizationUnitId))
                          .SelectMany(unit => unit.ThemeOrganizationUnits)
                          .Where(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                          .Select(link => link.Theme)
                          .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                          .Where(theme => theme.BeginDistribution <= beginDistributionDate
                                          && theme.EndDistribution >= endDistributionDate
                                          && !theme.IsDefault
                                          && !theme.ThemeTemplate.IsSkyScraper)
                          .Select(theme => new LinkingObjectsSchemaDto.ThemeDto
                                               {
                                                   Id = theme.Id,
                                                   Name = theme.Name
                                               })
                          .ToArray();
        }
    }
}
