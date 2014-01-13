using System;

using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Generic
{
    // FIXME {all, 23.10.2013}: в контексте scaleout требует рефакторинга - нужно state либо таскать на клиента, либо использовать shared persistence
    public class OperationSession
    {
        private const string DeactivateSessionCacheKeyTemplate = "deactivate-legalperson-{0}-byuser-{1}";
        private const string DeleteSessionCacheKeyTemplate = "delete-legalperson-{0}-byuser-{1}";

        private static readonly TimeSpan OperationSessionExpiration = TimeSpan.FromSeconds(10);
        private readonly string _operationSessionKey;

        public long Id { get; private set; }
        public bool CanProceedWithArchiveOrClosedOrders { get; set; }
        public bool CanProceedWithAccountDebts { get; set; }

        private OperationSession(long id, string operationSessionKey)
        {
            Id = id;
            _operationSessionKey = operationSessionKey; // на пустое значение проверим при закрытии сессии
        }

        public static OperationSession GetSession(ICacheAdapter cacheAdapter, BusinessOperation operation, long id, long userCode)
        {
            string operationSessionKey = null;
            switch (operation)
            {
                case BusinessOperation.Deactivate:
                    operationSessionKey = string.Format(DeactivateSessionCacheKeyTemplate, id, userCode);
                    break;
                case BusinessOperation.Delete:
                    operationSessionKey = string.Format(DeleteSessionCacheKeyTemplate, id, userCode);
                    break;
            }

            OperationSession operationSession;

            if (cacheAdapter.Contains(operationSessionKey))
            {
                operationSession = cacheAdapter.Get<OperationSession>(operationSessionKey);
            }
            else
            {
                operationSession = new OperationSession(id, operationSessionKey);
                cacheAdapter.Add(operationSessionKey, operationSession, OperationSessionExpiration);
            }

            return operationSession;
        }

        public void Close(ICacheAdapter cacheAdapter)
        {
            if (string.IsNullOrEmpty(_operationSessionKey) && cacheAdapter.Contains(_operationSessionKey))
            {
                cacheAdapter.Remove(_operationSessionKey);
            }
        } 
    }
}