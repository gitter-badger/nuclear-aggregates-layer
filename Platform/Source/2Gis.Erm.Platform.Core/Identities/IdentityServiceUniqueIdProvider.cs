using System;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Checkin;
using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Identities;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Identity;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class IdentityServiceUniqueIdProvider : IIdentityServiceUniqueIdProvider
    {
        private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);
        private readonly IIdentityServiceUniqueIdPersistenceService _identityServiceUniqueIdPersistenceService;
        private readonly IServiceInstanceIdProvider _serviceInstanceIdProvider;
        private readonly IApplicationLocksService _applicationLocksService;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly Lazy<byte> _lazyIdentityServiceUniqueId;

        public IdentityServiceUniqueIdProvider(IIdentityServiceUniqueIdPersistenceService identityServiceUniqueIdPersistenceService,
                                               IServiceInstanceIdProvider serviceInstanceIdProvider,
                                               IApplicationLocksService applicationLocksService,
                                               IGlobalizationSettings globalizationSettings)
        {
            _identityServiceUniqueIdPersistenceService = identityServiceUniqueIdPersistenceService;
            _serviceInstanceIdProvider = serviceInstanceIdProvider;
            _applicationLocksService = applicationLocksService;
            _globalizationSettings = globalizationSettings;
            _lazyIdentityServiceUniqueId = new Lazy<byte>(AcquireId);
        }

        private byte AcquireId()
        {
            Guid instanceId;
            if (!_serviceInstanceIdProvider.TryGetInstanceId(_timeout, out instanceId))
            {
                throw new InvalidOperationException("Can't get service instance id");
            }

            byte id;
            using (var appLock = _applicationLocksService.Acquire(LockName.ReserveIdentityServiceUniqueId, _timeout))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
                {
                    if (!_identityServiceUniqueIdPersistenceService.TryGetFirstIdleId((int)_globalizationSettings.BusinessModel, out id))
                    {
                        throw new InvalidOperationException(string.Format("Can't get unique id for service instance {0}", instanceId));
                    }

                    _identityServiceUniqueIdPersistenceService.ReserveId(id, instanceId);
                    transaction.Complete();
                }

                appLock.Release();
            }

            return id;
        }

        public byte GetUniqueId()
        {
            return _lazyIdentityServiceUniqueId.Value;
        }
    }
}