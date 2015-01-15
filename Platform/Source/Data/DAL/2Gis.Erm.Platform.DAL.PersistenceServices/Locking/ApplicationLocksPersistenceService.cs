using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking
{
    public class ApplicationLocksPersistenceService : IApplicationLocksPersistenceService
    {
        private readonly IDictionary<Guid, ApplicationLockDescriptor> _acquiredLocks = new Dictionary<Guid, ApplicationLockDescriptor>();
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IDatabaseCaller _databaseCaller;
        private readonly object _sync = new object();

        public ApplicationLocksPersistenceService(IDatabaseCaller databaseCaller, IConnectionStringSettings connectionStringSettings)
        {
            _databaseCaller = databaseCaller;
            _connectionStringSettings = connectionStringSettings;
        }

        public bool AcquireLock(string lockName, TimeSpan timeout, out Guid lockId)
        {
            SqlConnection connection = new SqlConnection(_connectionStringSettings.GetConnectionString(ConnectionStringName.Erm));
            using (var ts = new TransactionScope(TransactionScopeOption.Suppress))
            {
                connection.Open();
                ts.Complete();
            }

            var transaction = connection.BeginTransaction();
            var result = _databaseCaller.ExecuteProcedureWithReturnValue<LockAcquirementResult>("sys.sp_getapplock",
                                                                                                new
                                                                                                    {
                                                                                                        Resource = lockName,
                                                                                                        LockMode = "Exclusive",
                                                                                                        LockOwner = "Transaction",
                                                                                                        LockTimeout = timeout.TotalMilliseconds
                                                                                                    },
                                                                                                connection,
                                                                                                transaction);

            if (result == LockAcquirementResult.GrantedSynchronously || result == LockAcquirementResult.GrantedAfterWaiting)
            {
                lockId = Guid.NewGuid();
                lock (_sync)
                {
                    _acquiredLocks.Add(lockId, new ApplicationLockDescriptor(lockName, connection, transaction));
                }

                return true;
            }

            transaction.Commit();
            connection.Close();

            lockId = default(Guid);
            return false;
        }

        public bool IsLockActive(Guid lockId)
        {
            ApplicationLockDescriptor descriptor;
            if (!_acquiredLocks.TryGetValue(lockId, out descriptor))
            {
                return false;
            }

            if (!descriptor.Connection.State.HasFlag(ConnectionState.Open))
            {
                return false;
            }

            return _databaseCaller.QueryRawSql<int>("SELECT @@TRANCOUNT", null, descriptor.Connection, descriptor.Transaction).Single() != 0;
        }

        public bool ReleaseLock(Guid lockId)
        {
            lock (_sync)
            {
                ApplicationLockDescriptor lockDescriptor;
                if (!_acquiredLocks.TryGetValue(lockId, out lockDescriptor))
                {
                    return true;
                }

                var connection = lockDescriptor.Connection;
                var lockName = lockDescriptor.Name;
                var transaction = lockDescriptor.Transaction;

                try
                {
                    if (IsLockActive(lockId))
                    {
                        return false;
                    }

                    var result = _databaseCaller.ExecuteProcedureWithReturnValue<LockReleasingResult>("sys.sp_releaseapplock",
                                                                                                      new
                                                                                                          {
                                                                                                              Resource = lockName,
                                                                                                              LockOwner = "Transaction"
                                                                                                          },
                                                                                                      connection,
                                                                                                      transaction);

                    if (result == LockReleasingResult.Released)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                finally
                {
                    transaction.Dispose();
                    connection.Dispose();
                    _acquiredLocks.Remove(lockId);
                }
            }
        }

        #region nested

        private class ApplicationLockDescriptor
        {
            private readonly string _name;
            private readonly SqlConnection _connection;
            private readonly SqlTransaction _transaction;

            public ApplicationLockDescriptor(string name, SqlConnection connection, SqlTransaction transaction)
            {
                _name = name;
                _connection = connection;
                _transaction = transaction;
            }

            public string Name
            {
                get { return _name; }
            }

            public SqlConnection Connection
            {
                get { return _connection; }
            }

            public SqlTransaction Transaction
            {
                get { return _transaction; }
            }
        }

        #endregion
    }
}