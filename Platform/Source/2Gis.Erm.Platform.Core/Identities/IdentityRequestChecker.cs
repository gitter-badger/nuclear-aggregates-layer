using System;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class IdentityRequestChecker : IIdentityRequestChecker
    {
        public void CheckIdentityRequest(params Type[] entityTypes)
        {
            foreach (var type in entityTypes)
            {
                if (type.IsInstanceShared())
                {
                    throw new InvalidOperationException(string.Format("Can not generate identity for type {0}: it must be managed by user", type.Name));
                }
            }
        }
    }
}