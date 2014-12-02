using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old
{
    [Obsolete("Создан на время параллельного существования метаданных карточек в EntitySettings.xml и коде. " +
              "После отказа от EntitySettings.xml необходимо использовать ICardSettingsProvider")]
    public interface IUICardConfigurationService
    {
        CardStructure GetCardSettings(EntityName entityName, CultureInfo culture);
    }
}
