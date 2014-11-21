namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Абстракция для фабрики domaincontext, с учетом для какого host domain context создается domain context (конкретный UoWScope, либо сам UoW)
    /// </summary>
    public interface IModifiableDomainContextProviderForHost
    {
        IModifiableDomainContext Get<TEntity>(IDomainContextHost domainContextHost) where TEntity : class;
    }
}
