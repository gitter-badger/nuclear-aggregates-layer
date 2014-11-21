using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.References;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Processors.Concrete
{
    public sealed class ReferencesEvaluatorProcessor : IMetadataProcessor
    {
        public IMetadataKindIdentity[] TargetMetadataConstraints 
        {
            get { return new IMetadataKindIdentity[0]; }
        }

        public void Process(
            IMetadataKindIdentity metadataKind, 
            MetadataSet flattenedMetadata,
            MetadataSet concreteKindMetadata,
            IMetadataElement element)
        {
            bool hasReferences = false;
            var dereferencedChilds = new List<IMetadataElement>();
            foreach (var child in element.Elements)
            {
                var reference = child as MetadataReference;
                if (reference == null)
                {
                    dereferencedChilds.Add(child);
                    continue;
                }

                IMetadataElement metadataElement;
                if (!flattenedMetadata.Metadata.TryGetValue(reference.ReferencedElementId, out metadataElement))
                {
                    throw new InvalidOperationException("Can't resolve metadata for referenced element : " + reference.ReferencedElementId + ". References is ecounterred in " + reference.Parent.Identity.Id + " childs list");
                }

                hasReferences = true;
                flattenedMetadata.Metadata.Remove(reference.Identity.Id);
                concreteKindMetadata.Metadata.Remove(reference.Identity.Id);
                ((IMetadataElementUpdater)metadataElement).ReferencedBy(element);
                dereferencedChilds.Add(metadataElement);
            }

            if (!hasReferences)
            {
                return;
            }

            ((IMetadataElementUpdater)element).ReplaceChilds(dereferencedChilds);
        }
    }
}
