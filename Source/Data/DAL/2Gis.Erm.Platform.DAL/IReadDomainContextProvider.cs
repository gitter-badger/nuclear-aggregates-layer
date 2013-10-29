namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Абстракция для поставщика read domain context
    /// </summary>
    public interface IReadDomainContextProvider
    {
        IReadDomainContext Get();
    }
}
