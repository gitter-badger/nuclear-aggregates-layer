using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public abstract class ConfigElementBuilder<TBuilder, TConfigElement>
        where TConfigElement : ConfigElement
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
    {
        private readonly List<IConfigFeature> _features = new List<IConfigFeature>();
        private readonly List<ConfigElement> _childElements = new List<ConfigElement>();

        public static implicit operator TConfigElement(ConfigElementBuilder<TBuilder, TConfigElement> builder)
        {
            return Convert(builder);
        }

        public static implicit operator ConfigElement(ConfigElementBuilder<TBuilder, TConfigElement> builder)
        {
            return Convert(builder);
        }

        private static TConfigElement Convert(ConfigElementBuilder<TBuilder, TConfigElement> builder)
        {
            var element = builder.Create();
            if (!builder._childElements.Any())
            {
                return element;
            }

            ((IConfigElementUpdater)element).AddChilds(builder._childElements);
            foreach (var child in builder._childElements.OfType<IConfigElementUpdater>())
            {
                child.SetParent(element);
            }

            return element;
        }

        protected abstract TConfigElement Create();
        public ICollection<IConfigFeature> Features 
        {
            get
            {
                return _features;
            }
        }

        public TBuilder Childs(params ConfigElement[] childElements)
        {
            _childElements.AddRange(childElements);
            return ReturnBuilder();
        }

        public TBuilder WithFeatures(params IConfigFeature[] features)
        {
            _features.AddRange(features);
            return ReturnBuilder();
        }

        protected TBuilder ReturnBuilder()
        {
            return (TBuilder)this;
        }
    }
}
