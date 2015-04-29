namespace NuClear.Storage.Core
{
    /// <summary>
    /// Интерфейс фабрики для создания domain context
    /// </summary>
    /// <typeparam name="TDomainContext">The type of the domain context.</typeparam>
    public interface IDomainContextFactory<out TDomainContext> 
        where TDomainContext : IDomainContext
    {
        TDomainContext Create<TEntity>() where TEntity : class;
    }
}
