using System;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public interface ICardSettingsProvider
    {
        CardStructure GetCardSettings<TEntity>()
            where TEntity : IEntity;

        [Obsolete("После выпиливания метаданных из xml убрать этот метод")]
        CardStructure GetCardSettings(EntityName entity);
    }
}
