using System;

using DoubleGis.Erm.Qds.API.Operations.Documents;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;

namespace DoubleGis.Erm.Qds.Operations.Indexers.Raw
{
    public sealed class RawDocumentIndexer : IRawDocumentIndexer
    {
        // TODO: зарефакторить на mass processor
        private readonly IDocumentIndexer<UserDoc> _userDocIndexer;
        private readonly IDocumentIndexer<ClientGridDoc> _clientGridDocIndexer;

        public RawDocumentIndexer(IDocumentIndexer<UserDoc> userDocIndexer, IDocumentIndexer<ClientGridDoc> clientGridDocIndexer)
        {
            _userDocIndexer = userDocIndexer;
            _clientGridDocIndexer = clientGridDocIndexer;
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

            throw new InvalidOperationException();
        }
    }
}