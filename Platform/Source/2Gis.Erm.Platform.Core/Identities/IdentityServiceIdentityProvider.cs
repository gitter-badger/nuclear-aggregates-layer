using System;

using DoubleGis.Erm.Platform.API.Core.Identities;
using NuClear.IdentityService.Client.Interaction;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class IdentityServiceIdentityProvider : IdentityProviderBase
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;
        private readonly IIdentityServiceClient _identityServiceClient;

        public IdentityServiceIdentityProvider(IIdentityServiceClient identityServiceClient, IIdentityRequestChecker identityRequestChecker)
            : base(identityRequestChecker)
        {
            _identityServiceClient = identityServiceClient;
        }

        protected override long[] New(int count)
        {
            if (count > MaxRequestedCount)
            {
                throw new ArgumentException(string.Format("Cannot generate more than {0} ids at once", MaxRequestedCount), "count");
            }

            return _identityServiceClient.GetIdentities(count);
        }
    }
}