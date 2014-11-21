using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail
{
    /// <summary>
    /// Маркерный интерфейс для метаданных операции (любой)
    /// </summary>
    public interface IOperationMetadata
    {
    }

    /// <summary>
    /// Маркерный интерфейс для метаданных какой-то конкретной операции
    /// </summary>
    public interface IOperationMetadata<TOperationIdentity> : IOperationMetadata
        where TOperationIdentity : IOperationIdentity
    {
    }
}