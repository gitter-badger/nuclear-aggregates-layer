using DoubleGis.Erm.Platform.API.Core.Identities;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class NullIdentityProvider : IdentityProviderBase
    {
        public NullIdentityProvider(IIdentityRequestChecker checker) : base(checker)
        {
        }

        protected override long[] New(int count)
        {
            return new long[count];
        }
    }
}