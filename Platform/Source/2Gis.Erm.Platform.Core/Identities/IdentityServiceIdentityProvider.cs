using DoubleGis.Erm.Platform.API.Core.Identities;
using NuClear.IdentityService.Client.Interaction;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class IdentityServiceIdentityProvider : IdentityProviderBase
    {
        private readonly IIdentityServiceClient _identityServiceClient;

        public IdentityServiceIdentityProvider(IIdentityServiceClient identityServiceClient, IIdentityRequestChecker identityRequestChecker)
            : base(identityRequestChecker)
        {
            _identityServiceClient = identityServiceClient;
        }

        protected override long[] New(int count)
        {
            return _identityServiceClient.GetIdentities(count);
        }
    }
}