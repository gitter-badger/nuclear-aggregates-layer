namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationLoggingStrategy
    {
        LoggingSession Begin();
        bool TryLogUseCase(TrackedUseCase useCase, out string report);
        void Complete(LoggingSession loggingSession);
        void Close(LoggingSession loggingSession);
    }
}