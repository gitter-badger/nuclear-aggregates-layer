using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    public abstract class MetadataElementBuilder<TBuilder, TMetadataElement>
        where TMetadataElement : MetadataElement
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
    {
        private readonly List<IMetadataElement> _childElements = new List<IMetadataElement>();
        private IMetadataFeature[] _features = new IMetadataFeature[0];

        public IEnumerable<IMetadataFeature> Features 
        {
            get
            {
                return _features;
            }
        }

        public static implicit operator TMetadataElement(MetadataElementBuilder<TBuilder, TMetadataElement> builder)
        {
            return Convert(builder);
        }

        public static implicit operator MetadataElement(MetadataElementBuilder<TBuilder, TMetadataElement> builder)
        {
            return Convert(builder);
        }

        public TBuilder Childs(params IMetadataElement[] childElements)
        {
            _childElements.AddRange(childElements);
            return ReturnBuilder();
        }

        public TBuilder Childs(params MetadataElement[] childElements)
        {
            _childElements.AddRange(childElements);
            return ReturnBuilder();
        }

        public TBuilder WithFeatures(params IMetadataFeature[] features)
        {
            AddFeatures(features);
            return ReturnBuilder();
        }

        protected void AddFeatures(params IMetadataFeature[] features)
        {
            var uniqueFeatures = new HashSet<Type>();

            var mergedFeatures = 
                features
                    .Where(feature => !(feature is IUniqueMetadataFeature) || uniqueFeatures.Add(feature.GetType()))
                    .ToList();

            mergedFeatures.AddRange(_features.Where(f => !uniqueFeatures.Contains(f.GetType())));
            _features = mergedFeatures.ToArray();
        }

        protected abstract TMetadataElement Create();

        protected TBuilder ReturnBuilder()
        {
            return (TBuilder)this;
        }

        private static TMetadataElement Convert(MetadataElementBuilder<TBuilder, TMetadataElement> builder)
        {
            var element = builder.Create();
            if (!builder._childElements.Any())
            {
                return element;
            }

            ((IMetadataElementUpdater)element).AddChilds(builder._childElements);
            return element;
        }
    }
}
