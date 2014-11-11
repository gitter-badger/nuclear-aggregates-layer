using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFObjectContextFactory : IEFObjectContextFactory
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, DbCompiledModel> _dbModelCache = new Dictionary<string, DbCompiledModel>();

        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IEfDbModelFactory _efDbModelFactory;

        public EFObjectContextFactory(IConnectionStringSettings connectionStringSettings, IEfDbModelFactory efDbModelFactory)
        {
            _connectionStringSettings = connectionStringSettings;
            _efDbModelFactory = efDbModelFactory;
        }

        public ObjectContext CreateObjectContext(DomainContextMetadata domainContextMetadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(domainContextMetadata.ConnectionStringName);
            var sqlConnection = new SqlConnection(connectionString);

            var dbModel = GetDbModel(domainContextMetadata, sqlConnection);
           
            // TODO {a.tukaev, 11.11.2014}: возможно стоит использовать непосредственно DbContext через один из его конструкторов, вместо прослойки в виде ObjectContext
            return dbModel.CreateObjectContext<ObjectContext>(sqlConnection);
        }

        private DbCompiledModel GetDbModel(DomainContextMetadata contextMetadata, DbConnection sqlConnection)
        {
            DbCompiledModel dbModel;
            if (_dbModelCache.TryGetValue(contextMetadata.EntityContainerName, out dbModel))
            {
                return dbModel;
            }

            lock (_syncRoot)
            {
                if (_dbModelCache.TryGetValue(contextMetadata.EntityContainerName, out dbModel))
                {
                    return dbModel;
                }

                dbModel = _efDbModelFactory.Create(contextMetadata.EntityContainerName, sqlConnection);
                _dbModelCache.Add(contextMetadata.EntityContainerName, dbModel);
            }

            return dbModel;
        }
    }
}