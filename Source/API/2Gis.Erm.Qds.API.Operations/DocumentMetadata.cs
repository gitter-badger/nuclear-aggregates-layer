using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Qds.API.Operations.Documents;

namespace DoubleGis.Erm.Qds.API.Operations
{
    public sealed class DocumentMetadata
    {
        public static readonly Dictionary<Type, string> IndexNameMappings = new Dictionary<Type, string>
        {
            { typeof(MigrationDoc), "metadata" },
            { typeof(UserDoc), "metadata" },
            { typeof(ClientGridDoc), "data" },
        };

        public static Expression<Func<TDocument, object>> GetUserInputPropertyFor<TDocument>()
        {
            if (typeof(TDocument) == typeof(ClientGridDoc))
            {
                Expression<Func<TDocument, object>> f = x => (x as ClientGridDoc).Name;
                return f;
            }

            throw new ArgumentException();
        }
    }
}