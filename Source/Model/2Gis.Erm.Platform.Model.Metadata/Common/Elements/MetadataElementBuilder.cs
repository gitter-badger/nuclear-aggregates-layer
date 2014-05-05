using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    // TODO {i.maslennikov, 21.04.2014}: Предлагаю такой рефакторинг http://uk-tfs02.2gis.local:8080/tfs/DefaultCollection/_versionControl/shelveset?ss=MetadataElementBuilder%20encapsulation%3B2GIS%5Cd.ivanov
    // Суть - строгая инкапсуляция, сейчас в статическом методе Convert филд _childElements другого экземпляра доступен, хотя он private
    // COMMENT {all, 24.04.2014}: вариант в shelve возможный, однако, в нем чуть меньше инкапсуляция состояния, чем в текущем виде данного типа (т.к. свойство wrapper protected => доступно ещё и для подклассов)
    // сейчас, и field и метод Convert - это private members, единствнный корректный способ вызвать Convert - спровоцировать приведение типа => метод Convert работает не с другим экземпляром, а с единственно возможным, состояние которого и использует
    // возможный вариант - private wrapper IReadOnlyList, однако, создавать private тривиальный  accessor в этом случае (а этот тип - просто builder) как-то слишком
    // Итого пока решил, ничего не менять
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
