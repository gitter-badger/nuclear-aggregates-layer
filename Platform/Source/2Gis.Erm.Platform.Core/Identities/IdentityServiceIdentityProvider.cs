using System;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.NuClear.IdentityService.Client.Interaction;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class IdentityServiceIdentityProvider : IdentityProviderBase
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;
        private readonly IIdentityRequestStrategy _requestStrategy;

        public IdentityServiceIdentityProvider(IIdentityRequestStrategy requestStrategy, IIdentityRequestChecker identityRequestChecker)
            : base(identityRequestChecker)
        {
            _requestStrategy = requestStrategy;
        }

        protected override long[] New(int count)
        {
            if (count > MaxRequestedCount)
            {
                throw new ArgumentException(string.Format("Cannot generate more than {0} ids at once", MaxRequestedCount), "count");
            }

            return _requestStrategy.Request(count);
        }
    }
}