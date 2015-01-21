using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// Маркерный интерфейс для всех высокоуровневых операций Erm - все операции формируют множество applicationservices,
    /// используя которые можно обрабатывать агрегаты системы
    /// </summary>
    public interface IOperation
    {
    }

    /// <summary>
    /// Маркерный интерфейс. Назначение то же, что и у не generic версии + связь между интерфейсом операции и operation identity
    /// Любая интерфейс какой-то специфической операции ERM должен раширять именно эту (generic) версию - т.о. обеспечивая однозначеное соответсвие 
    /// операции и её identity
    /// </summary>
    public interface IOperation<TOperationIdentity> : IOperation
        where TOperationIdentity : IOperationIdentity, new()
    {
    }
}
