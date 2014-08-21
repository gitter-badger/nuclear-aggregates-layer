using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    /// <summary>
    /// Обеспечивает логирование набора изменений для указанного uscase
    /// </summary>
    public interface IOperationLogger
    {
        void Log(TrackedUseCase useCase);
    }
}
