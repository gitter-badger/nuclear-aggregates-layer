using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// Маркерный интерфейс для DTO специфичной для конкретной операции
    /// </summary>
    public interface IOperationSpecificEntityDto
    {
    }

    // TODO {all, 26.03.2014}: прописать для всех расширений/реализаций их реальные операции, а не просто маркер в виде IOperationSpecificEntityDto (т.е. без привязки к конкретной операции)
    public interface IOperationSpecificEntityDto<TOperationIdentity> : IOperationSpecificEntityDto
        where TOperationIdentity : class, IOperationIdentity
    {
    }
}
