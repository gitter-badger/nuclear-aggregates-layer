using System;

using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform;

namespace DoubleGis.Erm.Qds.Etl.Flow
{
    public class EtlFlow : IEtlFlow, IDataConsumer, ITransformedDataConsumer, IReferencesConsumer
    {
        private readonly ITransformation _transformation;
        private readonly IPublisher _publisher;
        private readonly IExtractor _extractor;
        readonly IReferencesBuilder _refBuilder;

        public EtlFlow(IPublisher publisher, ITransformation transformation, IExtractor extractor, IReferencesBuilder refBuilder)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException("publisher");
            }

            if (transformation == null)
            {
                throw new ArgumentNullException("transformation");
            }

            if (extractor == null)
            {
                throw new ArgumentNullException("extractor");
            }
            if (refBuilder == null)
            {
                throw new ArgumentNullException("refBuilder");
            }

            _publisher = publisher;
            _transformation = transformation;
            _extractor = extractor;
            _refBuilder = refBuilder;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            _refBuilder.BuildReferences(this);
        }

        public void ReferencesBuilt(IDataSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            _extractor.Extract(dataSource, this);
        }

        public void DataExtracted(IData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            _transformation.Transform(data, this);
        }

        public void DataTransformed(ITransformedData transformedData)
        {
            if (transformedData == null)
            {
                throw new ArgumentNullException("transformedData");
            }

            _publisher.Publish(transformedData);
        }
    }
}