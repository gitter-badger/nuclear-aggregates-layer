namespace NuClear.Storage.Core
{
    /// <summary>
    /// Абстракция для поставщика domaincontext допускающих модификацию данных
    /// </summary>
    public interface IModifiableDomainContextProvider
    {
        IModifiableDomainContext Get<TEntity>() where TEntity : class;
    }
}
