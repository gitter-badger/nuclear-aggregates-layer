using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail
{
    [DataContract(Name = "EmptyOperationMetadata_Of_{0}")]
    public sealed class EmptyOperationMetadata<TOperationIdentity> : IOperationMetadata<TOperationIdentity>
        where TOperationIdentity : IOperationIdentity
    {
    }

    public static class EmptyOperationMetadata
    {
        public static class Create
        {
            public static IOperationMetadata ForIdentityType(Type identityType)
            {
                if (!typeof(IOperationIdentity).IsAssignableFrom(identityType))
                {
                    throw new ArgumentException(identityType + " is not valid operation identity type");
                }

                return (IOperationMetadata)Activator.CreateInstance(typeof(EmptyOperationMetadata<>).MakeGenericType(identityType));
            }
        }
    }
}