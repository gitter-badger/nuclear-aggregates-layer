using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.Platform.DAL
{
    public class QueryProvider : IQueryProvider
    {
        private readonly IQuery _query;
        private readonly ISecureQuery _secureQuery;

        public QueryProvider(IQuery query, ISecureQuery secureQuery)
        {
            _query = query;
            _secureQuery = secureQuery;
        }

        public IQuery Get(IEntityType entityName)
        {
            return entityName.IsSecurableAccessRequired() ? _secureQuery : _query;
        } 
    }
}