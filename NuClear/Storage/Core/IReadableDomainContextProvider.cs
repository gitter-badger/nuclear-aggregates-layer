namespace NuClear.Storage.Core
{
    /// <summary>
    /// Абстракция для поставщика read domain context
    /// </summary>
    public interface IReadableDomainContextProvider
    {
        IReadableDomainContext Get();
    }
}
