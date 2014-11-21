using DoubleGis.Erm.Platform.API.Core.Identities;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class NullIdentityRequestStrategy : IIdentityRequestStrategy
    {
        public long[] Request(int count)
        {
            return new long[0];
        }
    }
}