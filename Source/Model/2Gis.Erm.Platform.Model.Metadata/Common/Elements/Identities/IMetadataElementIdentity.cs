using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities
{
    /// <summary>
    /// Базовый интерфейс для identity элемента метаданных, можно использовать для индентификации, маппинга и т.п. элементов метаданных
    /// Специфичные типы элементов метаданных, могут расширять интерфейс identity для более точной идентификации
    /// Т.о. любой элемент метаданных, должен иметь Id вида erm://metadata/Cards/Order, где префикс erm://metadata/Cards должен совпадать с префиксом (Id) рода метаданных, 
    /// к которому относится данный элемент
    /// </summary>
    public interface IMetadataElementIdentity : IEquatable<IMetadataElementIdentity>
    {
        Uri Id { get; }
    }
}
