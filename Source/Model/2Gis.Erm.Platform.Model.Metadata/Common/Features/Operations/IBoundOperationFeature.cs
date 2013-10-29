using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public interface IBoundOperationFeature : IConfigFeature
    {
        IOperationIdentity Identity { get; }
    }

    public interface IBoundOperationFeature<TOperationIdentity> : IBoundOperationFeature
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
    {
    }
}
