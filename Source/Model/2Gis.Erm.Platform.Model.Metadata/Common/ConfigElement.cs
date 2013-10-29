using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public abstract class ConfigElement : IConfigElement
    {
        public abstract IConfigElementIdentity ElementIdentity { get; }
        public abstract IEnumerable<IConfigFeature> ElementFeatures { get; }
        public abstract IConfigElement Parent { get; }
        public abstract int DeepLevel { get; }
        public abstract IConfigElement[] Elements { get; }
    }

    public abstract class ConfigElement<TElement, TElementIdentity, TBuilder> : ConfigElement, IConfigElement<TElementIdentity>, IConfigElementUpdater
        where TElement : ConfigElement<TElement, TElementIdentity, TBuilder>
        where TElementIdentity : class, IConfigElementIdentity
        where TBuilder : ConfigElementBuilder<TBuilder, TElement>, new()
    {
        public static TBuilder Config
        {
            get
            {
                return new TBuilder();
            }
        }
        
        private IEnumerable<IConfigFeature> _features;
        private IConfigElement _parent;
        private IConfigElement[] _childElements;
        
        protected ConfigElement(IEnumerable<IConfigFeature> features)
        {
            _childElements = new IConfigElement[0];
            _features = features;
        }
        
        public abstract TElementIdentity Identity { get; }

        public override IEnumerable<IConfigFeature> ElementFeatures 
        { 
            get
            {
                return _features;
            }
        }

        public override IConfigElement Parent 
        { 
            get
            {
                return _parent;
            } 
        }

        public override int DeepLevel
        {
            get
            {
                var parent = Parent;
                return parent != null ? parent.DeepLevel + 1 : 0;
            }
        }

        public override IConfigElement[] Elements 
        {
            get
            {
                return _childElements;
            }
        }

        void IConfigElementUpdater.SetParent(IConfigElement parentElement)
        {
            _parent = parentElement;
        }

        void IConfigElementUpdater.AddChilds(IEnumerable<IConfigElement> childs)
        {
            var mergedChilds = new List<IConfigElement>(_childElements);
            mergedChilds.AddRange(childs);
            _childElements = mergedChilds.ToArray();
        }

        void IConfigElementUpdater.AddFeature(IConfigFeature feature)
        {
            var mergedFeatures = new List<IConfigFeature>(_features) { feature };
            _features = mergedFeatures.ToArray();
        }
    }
}