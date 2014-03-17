using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFConnectionFactory : IEFConnectionFactory
    {
        private readonly object _syncRoot = new object();
        private readonly string[] _searchPaths = { "res://{0}/{1}.csdl", "res://{0}/{1}.ssdl", "res://{0}/{1}.msl" };
        private readonly Dictionary<string, MetadataWorkspace> _metadataWorkspaceCache = new Dictionary<string, MetadataWorkspace>();

        private readonly IConnectionStringSettings _connectionStringSettings;

        public EFConnectionFactory(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public EntityConnection CreateEntityConnection(DomainContextMetadata domainContextMetadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(domainContextMetadata.ConnectionStringName);
            var sqlConnection = new SqlConnection(connectionString);

            var metadataWorkspace = GetMetadataWorkspace(domainContextMetadata);
            var entityConnection = new EntityConnection(metadataWorkspace, sqlConnection);

            return entityConnection;
        }

        private MetadataWorkspace GetMetadataWorkspace(DomainContextMetadata contextMetadata)
        {
            MetadataWorkspace metadataWorkspace;
            if (_metadataWorkspaceCache.TryGetValue(contextMetadata.PathToEdmx, out metadataWorkspace))
            {
                return metadataWorkspace;
            }

            lock (_syncRoot)
            {
                if (_metadataWorkspaceCache.TryGetValue(contextMetadata.PathToEdmx, out metadataWorkspace))
                {
                    return metadataWorkspace;
                }

                var sb = new StringBuilder();

                var searchPaths = new string[_searchPaths.Length];
                for (var i = 0; i < _searchPaths.Length; i++)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, _searchPaths[i], contextMetadata.Assembly.FullName, contextMetadata.PathToEdmx);
                    searchPaths[i] = sb.ToString();
                    sb.Clear();
                }

                metadataWorkspace = new MetadataWorkspace(searchPaths, new[] { contextMetadata.Assembly });
                _metadataWorkspaceCache.Add(contextMetadata.PathToEdmx, metadataWorkspace);
            }

            return metadataWorkspace;
        }
    }
}