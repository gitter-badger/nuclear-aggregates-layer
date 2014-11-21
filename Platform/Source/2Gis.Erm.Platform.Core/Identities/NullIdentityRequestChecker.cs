using System;

using DoubleGis.Erm.Platform.API.Core.Identities;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class NullIdentityRequestChecker : IIdentityRequestChecker
    {
        public void CheckIdentityRequest(params Type[] entityTypes)
        {
        }
    }
}