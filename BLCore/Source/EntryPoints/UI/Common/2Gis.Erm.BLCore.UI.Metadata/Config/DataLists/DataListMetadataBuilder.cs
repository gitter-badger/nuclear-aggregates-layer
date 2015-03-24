using System;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Operations;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public sealed class DataListMetadataBuilder : MetadataElementBuilder<DataListMetadataBuilder, DataListMetadata>
    {
        private readonly OperationFeatureAspect<DataListMetadataBuilder, DataListMetadata> _operation;
        private readonly DataFieldsFeatureAspect<DataListMetadataBuilder, DataListMetadata> _dataFealds;
        private IEntityType _entityName;

        public DataListMetadataBuilder()
        {
            _dataFealds = new DataFieldsFeatureAspect<DataListMetadataBuilder, DataListMetadata>(this);
            _operation = new OperationFeatureAspect<DataListMetadataBuilder, DataListMetadata>(this);
        }

        public OperationFeatureAspect<DataListMetadataBuilder, DataListMetadata> Operation
        {
            get { return _operation; }
        }

        public DataFieldsFeatureAspect<DataListMetadataBuilder, DataListMetadata> DataFields
        {
            get { return _dataFealds; }
        }

        public DataListMetadataBuilder For(IEntityType entityName)
        {
            _entityName = entityName;
            return this;
        }

        public DataListMetadataBuilder BasedOn(DataListMetadata dataListMetadata)
        {
            // operations
            var batchOperationFeature = Features.OfType<OperationsSetFeature>().SingleOrDefault();
            if (batchOperationFeature == null)
            {
                batchOperationFeature = new OperationsSetFeature();
                WithFeatures(batchOperationFeature);
            }

            foreach (var feature in dataListMetadata.OperationFeatures)
            {
                batchOperationFeature.OperationFeatures.Add(feature);
            }

            // data fields
            var sourceDataFieldsFeature = dataListMetadata.Features.OfType<DataFieldsFeature>().SingleOrDefault();
            if (sourceDataFieldsFeature != null)
            {
                var destDataFieldsFeature = Features.OfType<DataFieldsFeature>().SingleOrDefault();
                if (destDataFieldsFeature == null)
                {
                    destDataFieldsFeature = new DataFieldsFeature(Enumerable.Empty<DataFieldMetadata>());
                    WithFeatures(destDataFieldsFeature);
                }

                foreach (var dataField in sourceDataFieldsFeature.DataFields)
                {
                    destDataFieldsFeature.DataFields.Add(dataField);
                }
            }

            // other
            foreach (var feature in dataListMetadata.Features)
            {
                if (Features.All(x => x.GetType() != feature.GetType()))
                {
                    WithFeatures(feature);
                }
            }

            if (_entityName.Equals(EntityType.Instance.None()))
            {
                _entityName = dataListMetadata.Entity;
            }

            return this;
        }

        public DataListMetadataBuilder DisplayNameLocalized(Expression<Func<string>> resourceKeyExpression)
        {
            WithFeatures(DisplayNameLocalizedFeature.Create(resourceKeyExpression));
            return this;
        }

        public DataListMetadataBuilder DefaultFilter(string defaultFilter)
        {
            WithFeatures(new DefaultFilterFeature());
            return this;
        }

        public DataListMetadataBuilder SortDescending()
        {
            WithFeatures(new SortDescendingFeature());
            return this;
        }

        protected override DataListMetadata Create()
        {
            var concreteList = Features.OfType<DisplayNameLocalizedFeature>().Single().ResourceKey;
            return new DataListMetadata(_entityName, concreteList, Features);
        }
    }
}