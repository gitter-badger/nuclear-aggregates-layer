using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DenormalizerTransformation : ITransformation
    {
        private readonly ErmEntitiesDenormalizer _denormalizer;
        private readonly IMetadataBinder _binder;
        private readonly IQdsComponentsFactory _qdsComponents;

        public DenormalizerTransformation(ErmEntitiesDenormalizer denormalizer, IMetadataBinder binder, IQdsComponentsFactory qdsComponentsesFactory)
        {
            if (denormalizer == null)
            {
                throw new ArgumentNullException("denormalizer");
            }
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }
            if (qdsComponentsesFactory == null)
            {
                throw new ArgumentNullException("qdsComponentsesFactory");
            }

            _denormalizer = denormalizer;
            _binder = binder;
            _qdsComponents = qdsComponentsesFactory;
        }

        public void Init()
        {
            _binder.BindMetadata(_qdsComponents.CreateQdsComponents());
        }

        public void Transform(IData data, ITransformedDataConsumer consumer)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var ermData = data as ErmData;
            if (ermData == null)
            {
                throw new ArgumentException(string.Format("Тип параметра должен быть {0}.", typeof(ErmData)), "data");
            }

            var documents = Transform(ermData);

            consumer.DataTransformed(new DenormalizedTransformedData(documents, ermData.State));
        }

        private IEnumerable<IDoc> Transform(ErmData data)
        {
            _denormalizer.ClearChangedDocuments();

            foreach (var entitySet in data.Data)
            {
                _denormalizer.DenormalizeByType(entitySet.EntityType, entitySet.Entities);
            }

            return _denormalizer.GetChangedDocuments();
        }
    }
}
