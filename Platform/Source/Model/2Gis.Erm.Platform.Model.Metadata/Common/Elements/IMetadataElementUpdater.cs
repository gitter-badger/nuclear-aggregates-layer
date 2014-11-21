using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    public interface IMetadataElementUpdater
    {
        void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity);
        void ActualizeKind(IMetadataKindIdentity actualMetadataKindIdentity);
        void SetParent(IMetadataElement parentElement);
        void ReferencedBy(IMetadataElement parentElement);
        void AddChilds(IEnumerable<IMetadataElement> childs);
        void ReplaceChilds(IEnumerable<IMetadataElement> childs);
        void AddFeature(IMetadataFeature feature);
    }
}