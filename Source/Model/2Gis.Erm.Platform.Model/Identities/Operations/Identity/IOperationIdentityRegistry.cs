using System;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public interface IOperationIdentityRegistry
    {
        IOperationIdentity[] Identities { get; }
        TOperationIdentity GetIdentity<TOperationIdentity>() 
            where TOperationIdentity : IOperationIdentity;
        IOperationIdentity GetIdentity(Type identityType);
        IOperationIdentity GetIdentity(int operationId);
    }
}