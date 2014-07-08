namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationLoggingStrategy
    {
        bool TryLogUseCase(TrackedUseCase useCase, out string report);
    }
}