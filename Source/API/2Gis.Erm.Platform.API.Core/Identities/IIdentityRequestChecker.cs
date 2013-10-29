using System;

namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    public interface IIdentityRequestChecker
    {
        void CheckIdentityRequest(params Type[] entityTypes);
    }
}