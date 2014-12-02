using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old
{
    public interface IUIConfigurationService
    {
        IEnumerable<NavigationElementStructure> GetNavigationSettings(CultureInfo culture);
        EntityDataListsContainer GetGridSettings(EntityName entityName, CultureInfo culture);
    }
}
