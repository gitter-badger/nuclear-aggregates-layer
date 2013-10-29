using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    /// <summary>
    /// Базовый интерфейс для identity элемента конфигурации, можно использовать для индентификации, маппинга и т.п. элементов конфигурации
    /// Специфичные типы элементов конфигурации, могут расширять интерфейс identity для более точной идентификации
    /// </summary>
    public interface IConfigElementIdentity : IEquatable<IConfigElementIdentity>
    {
        int Id { get; }
    }
}
