using System;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Checkin;
using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Identities;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Identity;

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

        public byte GetUniqueId()
        {
            var identityServiceId = _lazyIdentityServiceUniqueId.Value;
            CheckIdReserved(identityServiceId, _serviceInstanceIdProvider.GetInstanceId(_timeout));
            return identityServiceId;
        }

        private byte AcquireId()
        {
            Guid instanceId;
            if (!_serviceInstanceIdProvider.TryGetInstanceId(_timeout, out instanceId))
            {
                throw new InvalidOperationException("Can't get service instance id");
            }

            byte id;
            using (var appLock = _applicationLocksService.Acquire(LockName.ReserveIdentityServiceUniqueId, LockOwner.Session, _timeout))
            {
                if (!_identityServiceUniqueIdPersistenceService.TryGetFirstIdleId((int)_globalizationSettings.BusinessModel, out id))
                {
                    throw new InvalidOperationException(string.Format("Can't get unique id for service instance {0}", instanceId));
                }

                _identityServiceUniqueIdPersistenceService.ReserveId(id, instanceId);

                appLock.Release();
            }

            return id;
        }

        private void CheckIdReserved(byte identityServiceId, Guid serviceInstanceId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                // ловим сразу два кейса - поменялся instance id (закрыли service host) либо кто-то занял id, закрепленный за текущим instance id
                if (!_identityServiceUniqueIdPersistenceService.IsIdReservedBy(identityServiceId, serviceInstanceId))
                {
                    throw new InvalidOperationException(string.Format("Identity service unique id {0} is not reserved by service instance {1}", identityServiceId, serviceInstanceId));
                }

                transaction.Complete();
            }
        }
    }
}