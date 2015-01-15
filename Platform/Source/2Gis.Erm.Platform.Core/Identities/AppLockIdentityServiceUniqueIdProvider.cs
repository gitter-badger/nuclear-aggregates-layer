using System;

using DoubleGis.Erm.Platform.Common.Identities;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class AppLockIdentityServiceUniqueIdProvider : IIdentityServiceUniqueIdProvider
    {
        private const string IdAppLockTemplate = "Id={0}";

        private readonly IApplicationLocksPersistenceService _applicationLocksPersistenceService;
        private readonly Random _rnd = new Random();

        private byte? _id;
        private Guid? _lockId;


        public AppLockIdentityServiceUniqueIdProvider(IApplicationLocksPersistenceService applicationLocksPersistenceService)
        {
            _applicationLocksPersistenceService = applicationLocksPersistenceService;
        }

        public byte GetUniqueId()
        {
            EnsureIdReserved();
            // ReSharper disable once PossibleInvalidOperationException
            return _id.Value;
        }

        private void EnsureIdReserved()
        {
            if (_id != null && _lockId != null && _applicationLocksPersistenceService.IsLockActive(_lockId.Value))
            {
                return;
            }

            var startId = _id ?? (byte)_rnd.Next(255);
            for (var i = 0; i < 255; i++)
            {
                var id = (byte)((startId + i) % 256);
                Guid lockId;
                if (_applicationLocksPersistenceService.AcquireLock(string.Format(IdAppLockTemplate, id), TimeSpan.Zero, out lockId))
                {
                    _lockId = lockId;
                    _id = id;
                    return;
                }
            }

            throw new InvalidOperationException("Can't acquire id appliaction lock for identity service");
        }
    }
}