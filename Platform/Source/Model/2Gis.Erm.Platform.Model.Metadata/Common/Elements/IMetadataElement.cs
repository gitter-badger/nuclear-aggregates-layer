using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    /// <summary>
    /// Базовый интерфейс элемента метаданных, таким элементом может быть,
    /// сущность ERM, элемент иерархии, карточка, документ и др.
    /// </summary>
    public interface IMetadataElement
    {
        IMetadataElementIdentity Identity { get; }
        IMetadataKindIdentity Kind { get; }
        IEnumerable<IMetadataFeature> Features { get; }

        IMetadataElement Parent { get; }
        IMetadataElement[] References { get; }
        int DeepLevel { get; }
        IMetadataElement[] Elements { get; }
    }
}
