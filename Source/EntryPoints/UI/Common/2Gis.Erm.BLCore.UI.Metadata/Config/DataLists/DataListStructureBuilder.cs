using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataListStructureBuilder : ConfigElementBuilder<DataListStructureBuilder, DataListStructure>
    {
        private readonly OperationBoundFeatureAspect<DataListStructureBuilder, DataListStructure> _operation;
        private readonly DataFieldsFeatureAspect<DataListStructureBuilder, DataListStructure> _dataFealds;
        private EntityName _entityName;

        public DataListStructureBuilder()
        {
            _dataFealds = new DataFieldsFeatureAspect<DataListStructureBuilder, DataListStructure>(this);
            _operation = new OperationBoundFeatureAspect<DataListStructureBuilder, DataListStructure>(this);
        }
        public OperationBoundFeatureAspect<DataListStructureBuilder, DataListStructure> Operation
        {
            get { return _operation; }
        }

        public DataFieldsFeatureAspect<DataListStructureBuilder, DataListStructure> DataFields
        {
            get { return _dataFealds; }
        }

        public DataListStructureBuilder For(EntityName entityName)
        {
            _entityName = entityName;
            return this;
        }

        public DataListStructureBuilder BasedOn(DataListStructure dataListStructure)
        {
            // operations
            var batchOperationFeature = Features.OfType<BatchOperationFeature>().SingleOrDefault();

            if (batchOperationFeature == null)
            {
                batchOperationFeature = new BatchOperationFeature();
                Features.Add(batchOperationFeature);
            }

            foreach (var feature in dataListStructure.OperationFeatures)
            {
                batchOperationFeature.OperationFeatures.Add(feature);
            }


            // data fields
            var sourceDataFieldsFeature = dataListStructure.ElementFeatures.OfType<DataFieldsFeature>().SingleOrDefault();
            if (sourceDataFieldsFeature != null)
            {
                var destDataFieldsFeature = Features.OfType<DataFieldsFeature>().SingleOrDefault();
                if (destDataFieldsFeature == null)
                {
                    destDataFieldsFeature = new DataFieldsFeature(Enumerable.Empty<DataFieldStructure>());
                    Features.Add(destDataFieldsFeature);
                }

                foreach (var dataField in sourceDataFieldsFeature.DataFields)
                {
                    destDataFieldsFeature.DataFields.Add(dataField);
                }
            }

            // other
            foreach (var feature in dataListStructure.ElementFeatures)
            {
                if (Features.All(x => x.GetType() != feature.GetType()))
                {
                    Features.Add(feature);
                }
            }

            if (_entityName == EntityName.None)
            {
                _entityName = dataListStructure.Identity.EntityName;
            }

            return this;
        }

        public DataListStructureBuilder DisplayNameLocalized(Expression<Func<string>> resourceKeyExpression)
        {
            Features.Add(DisplayNameLocalizedFeature.Create(resourceKeyExpression));
            return this;
        }

        public DataListStructureBuilder DefaultFilter(string defaultFilter)
        {
            Features.Add(new DefaultFilterFeature());
            return this;
        }

        public DataListStructureBuilder SortDescending()
        {
            Features.Add(new SortDescendingFeature());
            return this;
        }

        protected override DataListStructure Create()
        {
            
            return new DataListStructure(_entityName, Features);
        }
    }
}