using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Themes.ReadModel
{
    public interface IThemeReadModel : IAggregateReadModel<Theme>
    {
        IEnumerable<LinkingObjectsSchemaDto.ThemeDto> FindThemesCanBeUsedWithOrder(long destOrganizationUnitId, DateTime beginDistributionDate, DateTime endDistributionDate);
    }
}