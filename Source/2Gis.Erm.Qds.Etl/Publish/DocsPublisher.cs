using System;

using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Publish
{
    public class DocsPublisher : IPublisher
    {
        private readonly IDocsStorage _docsStorage;
        private readonly IChangesTrackerState _changesTrackerState;

        public DocsPublisher(IDocsStorage docsStorage, IChangesTrackerState changesTrackerState)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }
            if (changesTrackerState == null)
            {
                throw new ArgumentNullException("changesTrackerState");
            }

            _docsStorage = docsStorage;
            _changesTrackerState = changesTrackerState;
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

            _docsStorage.Update(documents.Documents);
            _changesTrackerState.SetState(documents.State);
            _docsStorage.Flush();
        }
    }
}