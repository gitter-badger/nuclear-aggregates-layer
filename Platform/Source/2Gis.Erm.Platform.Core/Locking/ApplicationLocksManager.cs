using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking;

namespace DoubleGis.Erm.Platform.Core.Locking
{
    public class ApplicationLocksManager : IApplicationLocksManager
    {
        private readonly IDictionary<Guid, ApplicationLockDescriptor> _acquiredLocks = new Dictionary<Guid, ApplicationLockDescriptor>();
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IDatabaseCaller _databaseCaller;
        private readonly object _sync = new object();

        public ApplicationLocksManager(IDatabaseCaller databaseCaller, IConnectionStringSettings connectionStringSettings)
        {
            _databaseCaller = databaseCaller;
            _connectionStringSettings = connectionStringSettings;
        }

        public bool AcquireLock(string lockName, LockOwner lockOwner, LockScope lockScope, TimeSpan timeout, out Guid lockId)
        {
            var connectionString = lockScope == LockScope.CurrentInstallation ? ConnectionStringName.Erm : ConnectionStringName.ErmInfrastructure;
            var builder = new SqlConnectionStringBuilder(_connectionStringSettings.GetConnectionString(connectionString)) { Pooling = false, Enlist = false };
            var connection = new SqlConnection(builder.ConnectionString);

            connection.Open();
            var transaction = lockOwner == LockOwner.Transaction ? connection.BeginTransaction(IsolationLevel.ReadUncommitted) : null;
            var result = _databaseCaller.ExecuteProcedureWithReturnValue<LockAcquirementResult>("sys.sp_getapplock", 
                                                                                                new
                                                                                                    {
                                                                                                        Resource = lockName, 
                                                                                                        LockMode = "Exclusive", 
                                                                                                        LockOwner = lockOwner.ToString(), 
                                                                                                        LockTimeout = timeout.TotalMilliseconds
                                                                                                    }, 
                                                                                                connection, 
                                                                                                transaction);

            if (result == LockAcquirementResult.GrantedSynchronously || result == LockAcquirementResult.GrantedAfterWaiting)
            {
                lockId = Guid.NewGuid();
                lock (_sync)
                {
                    _acquiredLocks.Add(lockId, new ApplicationLockDescriptor(lockOwner, connection, transaction));
                }

                return true;
            }

            if (transaction != null)
            {
                transaction.Commit();
            }

            connection.Close();

            lockId = default(Guid);
            return false;
        }

        public bool IsLockActive(Guid lockId)
        {
            ApplicationLockDescriptor descriptor;
            lock (_sync)
            {
                if (!_acquiredLocks.TryGetValue(lockId, out descriptor))
                {
                    return false;
                }
            }

            if (!descriptor.Connection.State.HasFlag(ConnectionState.Open))
            {
                return false;
            }

            if (descriptor.LockOwner == LockOwner.Session)
            {
                return true;
            }

            return _databaseCaller.QueryRawSql<int>("SELECT @@TRANCOUNT", null, descriptor.Connection, descriptor.Transaction).Single() == 1;
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
                var transaction = lockDescriptor.Transaction;

                try
                {
                    if (IsLockActive(lockId))
                    {
                        return false;
                    }

                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    connection.Close();
                    return true;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }

                    connection.Dispose();
                    _acquiredLocks.Remove(lockId);
                }
            }
        }

        #region nested

        private class ApplicationLockDescriptor
        {
            private readonly LockOwner _lockOwner;
            private readonly SqlConnection _connection;
            private readonly SqlTransaction _transaction;

            public ApplicationLockDescriptor(LockOwner lockOwner, SqlConnection connection, SqlTransaction transaction)
            {
                _lockOwner = lockOwner;
                _connection = connection;
                _transaction = transaction;
            }

            public SqlConnection Connection
            {
                get { return _connection; }
            }

            public SqlTransaction Transaction
            {
                get { return _transaction; }
            }

            public LockOwner LockOwner
            {
                get { return _lockOwner; }
            }
        }

        #endregion
    }
}