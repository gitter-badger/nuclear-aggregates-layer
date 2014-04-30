using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Processors
{
    public abstract class MetadataProcessorBase<TMetadataKindIdentity, TMetadataElement> : IMetadataProcessor 
        where TMetadataKindIdentity : class, IMetadataKindIdentity, new()
        where TMetadataElement : class, IMetadataElement
    {
        private readonly TMetadataKindIdentity _metadataKindIdentity = new TMetadataKindIdentity();

        public IMetadataKindIdentity[] TargetMetadataConstraints
        {
            get { return new IMetadataKindIdentity[] { _metadataKindIdentity }; }
        }

        public void Process(
            IMetadataKindIdentity metadataKind, 
            MetadataSet flattenedMetadata, 
            MetadataSet concreteKindMetadata, 
            IMetadataElement element)
        {
            var typedElement = element as TMetadataElement;
            if (typedElement == null)
            {
                return;
            }

            Process(metadataKind, flattenedMetadata, concreteKindMetadata, typedElement);
        }

        protected abstract void Process(
            IMetadataKindIdentity metadataKind, 
            MetadataSet flattenedMetadata, 
            MetadataSet concreteKindMetadata,
            TMetadataElement metadata);
    }
}