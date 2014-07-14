using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IVerifierContext
    {
        TrackedUseCase UseCase { get; }
    }
}