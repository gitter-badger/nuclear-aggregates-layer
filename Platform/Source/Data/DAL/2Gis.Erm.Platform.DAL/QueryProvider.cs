using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.Platform.DAL
{
    public class QueryProvider : IQueryProvider
    {
        private readonly IQuery _query;
        private readonly ISecureFinder _secureFinder;

        public QueryProvider(IQuery query, ISecureFinder secureFinder)
        {
            _query = query;
            _secureFinder = secureFinder;
        }

        public IQuery Get(IEntityType entityName)
        {
            return entityName.IsSecurableAccessRequired() ? _secureFinder : _query;
        } 
    }
}