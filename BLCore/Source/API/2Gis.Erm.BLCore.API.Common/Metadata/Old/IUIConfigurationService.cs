using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old
{
    public interface IUIConfigurationService
    {
        IEnumerable<NavigationElementStructure> GetNavigationSettings(CultureInfo culture);
        EntityDataListsContainer GetGridSettings(IEntityType entityName, CultureInfo culture);
        CardStructure GetCardSettings(IEntityType entityName, CultureInfo culture);
    }
}
