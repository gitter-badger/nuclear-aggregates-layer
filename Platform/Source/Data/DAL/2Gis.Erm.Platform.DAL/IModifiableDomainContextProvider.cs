namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Абстракция для поставщика domaincontext допускающих модификацию данных
    /// </summary>
    public interface IModifiableDomainContextProvider
    {
        IModifiableDomainContext Get<TEntity>() where TEntity : class;
    }
}
