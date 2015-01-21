using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.UI.Metadata.Operations
{
    /// <summary>
    /// Маркерный интерфейс операции, которая существует только в клиентских частях ERM и отсутсвует на серверной стороне (например, закрыть карточку, обновить и т.п.)
    /// </summary>
    public interface IClientOperationIdentity : IOperationIdentity
    {
    }
}
