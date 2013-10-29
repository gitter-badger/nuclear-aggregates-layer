using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    /// <summary>
    /// Обеспечивает логирование набора изменений для указанной иерархии operationscopes
    /// </summary>
    public interface IOperationLogger : ISimplifiedModelConsumer
    {
        void Log(OperationScopeNode scopeNode);
    }
}
