using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Identity
{
    public class IdentityServiceUniqueIdPersistenceService : IIdentityServiceUniqueIdPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public IdentityServiceUniqueIdPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public bool TryGetFirstIdleId(int installationId, out byte id)
        {
            var results = _databaseCaller.QueryRawSql<byte>(@"SELECT TOP 1 [isi].[IdentityServiceUniqueId]
                                                FROM [Metadata].[IdentityServiceIds] [isi]
                                                LEFT JOIN [Metadata].[ServiceInstances] [si] ON [si].[Id] = [isi].[ServiceInstanceId]
                                                WHERE [isi].[InstallationId] = @InstallationId AND ([si].[Id] IS NULL OR [si].[IsRunning] = 0)",
                                                            new
                                                                {
                                                                    InstallationId = installationId
                                                                })
                                         .ToArray();

            if (results.Length == 1)
            {
                id = results[0];
                return true;
            }

            id = 0;
            return false;
        }

        public void ReserveId(byte id, Guid serviceInstanceId)
        {
                _databaseCaller.ExecuteRawSql(@"UPDATE [Metadata].[IdentityServiceIds]
                                            SET [ServiceInstanceId] = @ServiceInstanceId
                                            WHERE [IdentityServiceUniqueId] = @IdentityServiceUniqueId",
                                              new { IdentityServiceUniqueId = id, ServiceInstanceId = serviceInstanceId });
               
        }
    }
}