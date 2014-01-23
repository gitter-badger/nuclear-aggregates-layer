using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;

namespace DoubleGis.Erm.Qds.Operations.Indexers.Raw
{
    public sealed class RawEntityIndexer : IRawEntityIndexer
    {
        // TODO: зарефакторить на mass processor
        private readonly IEntityIndexer<User> _userIndexer;
        private readonly IEntityIndexer<Client> _clientEntityIndexer;

        public RawEntityIndexer(IEntityIndexer<User> userIndexer, IEntityIndexer<Client> clientEntityIndexer)
        {
            _userIndexer = userIndexer;
            _clientEntityIndexer = clientEntityIndexer;
        }

        void IRawEntityIndexer.IndexEntities(string entityType, long[] ids)
        {
            if (string.Equals(entityType, "User", StringComparison.OrdinalIgnoreCase))
            {
                _userIndexer.IndexEntities(ids);
                return;
            }

            if (string.Equals(entityType, "Client", StringComparison.OrdinalIgnoreCase))
            {
                _clientEntityIndexer.IndexEntities(ids);
                return;
            }

            throw new InvalidOperationException();
        }
    }
}