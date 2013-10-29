using System;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public sealed class NonCoupledOperationFeature<TOperationIdentity> : IBoundOperationFeature<TOperationIdentity>
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
    {
        private readonly static Lazy<NonCoupledOperationFeature<TOperationIdentity>> SingleInstance =
            new Lazy<NonCoupledOperationFeature<TOperationIdentity>>(() => new NonCoupledOperationFeature<TOperationIdentity>());

        public static NonCoupledOperationFeature<TOperationIdentity> Instance
        {
            get
            {
                return SingleInstance.Value;
            }
        }

        public IOperationIdentity Identity
        {
            get
            {
                return new TOperationIdentity();
            }
        }
    }
}