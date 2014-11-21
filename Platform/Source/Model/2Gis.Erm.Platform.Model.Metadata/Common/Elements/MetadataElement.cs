using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    public abstract class MetadataElement : IMetadataElement, IMetadataElementUpdater
    {
        private IMetadataKindIdentity _kind;
        private IEnumerable<IMetadataFeature> _features;
        private IMetadataElement _parent;
        private IMetadataElement[] _references = new IMetadataElement[0];
        private IMetadataElement[] _childElements = new IMetadataElement[0];

        protected MetadataElement(IEnumerable<IMetadataFeature> features)
        {
            _childElements = new IMetadataElement[0];
            _features = features;
        }

        public abstract IMetadataElementIdentity Identity { get; }

        public IMetadataKindIdentity Kind
        {
            get { return _kind; }
        }

        public IEnumerable<IMetadataFeature> Features
        {
            get
            {
                return _features;
            }
        }

        public IMetadataElement Parent
        {
            get
            {
                return _parent;
            }
        }

        public IMetadataElement[] References
        {
            get { return _references; }
        }

        public int DeepLevel
        {
            get
            {
                var parent = Parent;
                return parent != null ? parent.DeepLevel + 1 : 0;
            }
        }

        public IMetadataElement[] Elements
        {
            get
            {
                return _childElements;
            }
        }

        public abstract void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity);
        
        public override string ToString()
        {
            return string.Format("Type:{0}. Kind:{1}. Id:{2}", GetType().Name, _kind != null ? _kind.Id.ToString() : "UNKNOWN", Identity.Id);
        }

        void IMetadataElementUpdater.ActualizeKind(IMetadataKindIdentity actualMetadataKindIdentity)
        {
            _kind = actualMetadataKindIdentity;
        }

        void IMetadataElementUpdater.SetParent(IMetadataElement parentElement)
        {
            _parent = parentElement;
        }

        void IMetadataElementUpdater.ReferencedBy(IMetadataElement referenceElement)
        {
            _references = new List<IMetadataElement>(_references) { referenceElement }.ToArray();
        }

        void IMetadataElementUpdater.AddChilds(IEnumerable<IMetadataElement> childs)
        {
            var mergedChilds = new List<IMetadataElement>(_childElements);
            mergedChilds.AddRange(ActualizeParent(childs.ToArray()));
            _childElements = mergedChilds.ToArray();
        }

        void IMetadataElementUpdater.ReplaceChilds(IEnumerable<IMetadataElement> childs)
        {
            _childElements = childs.ToArray();
        }

        void IMetadataElementUpdater.AddFeature(IMetadataFeature feature)
        {
            var mergedFeatures = new List<IMetadataFeature>(_features) { feature };
            _features = mergedFeatures.ToArray();
        }

        private IEnumerable<IMetadataElement> ActualizeParent(IEnumerable<IMetadataElement> childs)
        {
            var childsSnapshot = childs.ToArray();
            foreach (var child in childsSnapshot)
            {
                var childUpdater = (IMetadataElementUpdater)child;
                childUpdater.SetParent(this);
            }

            return childsSnapshot;
        }
    }

    public abstract class MetadataElement<TElement, TBuilder> : MetadataElement
        where TElement : MetadataElement<TElement, TBuilder>
        where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new()
    {
        protected MetadataElement(IEnumerable<IMetadataFeature> features)
            : base(features)
        {
        }
        
        public static TBuilder Config
        {
            get
            {
                return new TBuilder();
            }
        }
    }
}