using System;

using Nest;

namespace DoubleGis.Erm.Qds.Common.ElasticClient
{
    public interface IElasticConnectionSettingsFactory
    {
        IConnectionSettings CreateConnectionSettings(Uri uri);

        string GetIsolatedIndexName(string indexName);
    }
}
