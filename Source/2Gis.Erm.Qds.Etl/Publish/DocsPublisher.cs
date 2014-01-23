using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Publish
{
    public class DocsPublisher : IPublisher
    {
        private readonly IDocsStorage _docsStorage;

        public DocsPublisher(IDocsStorage docsStorage)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            _docsStorage = docsStorage;
        }

        public void Publish(ITransformedData transformedData)
        {
            if (transformedData == null)
            {
                throw new ArgumentNullException("transformedData");
            }

            var documents = transformedData as DenormalizedTransformedData;
            if (documents == null)
            {
                throw new ArgumentException(string.Format("Тип параметра должен быть {0}.", typeof(DenormalizedTransformedData)), "transformedData");
            }

            Publish(documents.Documents);
            Publish(new[] { (IDoc)documents.State });
        }

        void Publish(IEnumerable<IDoc> docs)
        {
            _docsStorage.Update(docs);
        }
    }
}