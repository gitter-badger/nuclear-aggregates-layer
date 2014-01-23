using System;
using System.Collections;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DenormalizerTransformation : ITransformation
    {
        private readonly ErmEntitiesDenormalizer _denormalizer;

        public DenormalizerTransformation(ErmEntitiesDenormalizer denormalizer)
        {
            if (denormalizer == null)
            {
                throw new ArgumentNullException("denormalizer");
            }

            _denormalizer = denormalizer;
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
            foreach (var entitySet in data.Data)
            {
                _denormalizer.DenormalizeByType(entitySet.EntityType, entitySet.Entities);
            }

            return _denormalizer.GetChangedDocuments();
        }
    }
}
