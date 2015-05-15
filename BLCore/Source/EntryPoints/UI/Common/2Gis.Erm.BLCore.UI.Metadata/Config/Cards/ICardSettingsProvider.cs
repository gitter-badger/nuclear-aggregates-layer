using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public interface ICardSettingsProvider
    {
        // TODO {all, 24.12.2014}: Культура передается т.к. на старте приложения проверяется корректность метаданных.
        // Контекст пользователя в это время неопределен. После удаления метаданных карточек из EntitySettings.xml и соответсвующей проверки метаданных карточек
        // культуру можно будет брать из контекста пользователя
        CardStructure GetCardSettings<TEntity>(CultureInfo culture)
            where TEntity : IEntity;

        [Obsolete("После выпиливания метаданных из xml убрать этот метод")]
        CardStructure GetCardSettings(IEntityType entity, CultureInfo culture);
    }
}
