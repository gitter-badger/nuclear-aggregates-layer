using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    /// <summary>
    /// Базовый интерфейс элемента конфигурации, таким элементом может быть,
    /// сущность ERM, элемент иерархии, карточка, документ и др.
    /// </summary>
    public interface IConfigElement
    {
        IConfigElementIdentity ElementIdentity { get; }
        IEnumerable<IConfigFeature> ElementFeatures { get; }

        IConfigElement Parent { get; }
        int DeepLevel { get; }
        IConfigElement[] Elements { get; }
    }

    public interface IConfigElement<TIdentity> : IConfigElement
        where TIdentity : class, IConfigElementIdentity
    {
        TIdentity Identity { get; }
    }
}
