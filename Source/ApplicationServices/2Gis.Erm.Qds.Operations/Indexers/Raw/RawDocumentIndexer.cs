using System;

using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

namespace DoubleGis.Erm.Qds.Operations.Indexers.Raw
{
    public sealed class RawDocumentIndexer : IRawDocumentIndexer
    {
        // TODO: зарефакторить на mass processor
        private readonly IDocumentIndexer<UserDoc> _userDocIndexer;
        private readonly IDocumentIndexer<TerritoryDoc> _territoryDocIndexer;
        private readonly IDocumentIndexer<RecordIdState> _recordIdStateIndexer;
        private readonly IDocumentIndexer<ClientGridDoc> _clientGridDocIndexer;

        public RawDocumentIndexer(IDocumentIndexer<UserDoc> userDocIndexer, IDocumentIndexer<ClientGridDoc> clientGridDocIndexer, IDocumentIndexer<TerritoryDoc> territoryDocIndexer, IDocumentIndexer<RecordIdState> recordIdStateIndexer)
        {
            _userDocIndexer = userDocIndexer;
            _clientGridDocIndexer = clientGridDocIndexer;
            _territoryDocIndexer = territoryDocIndexer;
            _recordIdStateIndexer = recordIdStateIndexer;
        }

        void IRawDocumentIndexer.IndexAllDocuments(string documentType)
        {
            if (string.Equals(documentType, "UserDoc", StringComparison.OrdinalIgnoreCase))
            {
                _userDocIndexer.IndexAllDocuments();
                return;
            }

            if (string.Equals(documentType, "ClientGridDoc", StringComparison.OrdinalIgnoreCase))
            {
                _clientGridDocIndexer.IndexAllDocuments();
                return;
            }

            if (string.Equals(documentType, "TerritoryDoc", StringComparison.OrdinalIgnoreCase))
            {
                _territoryDocIndexer.IndexAllDocuments();
                return;
            }

            if (string.Equals(documentType, "RecordIdState", StringComparison.OrdinalIgnoreCase))
            {
                _recordIdStateIndexer.IndexAllDocuments();
                return;
            }

            throw new InvalidOperationException();
        }
    }
}