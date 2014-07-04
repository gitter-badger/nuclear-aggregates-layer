using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    /// <summary>
    /// Обеспечивает логирование набора изменений для указанной иерархии operationscopes
    /// </summary>
    public interface IOperationLogger
    {
        void Log(OperationScopeNode scopeNode);
    }
}
