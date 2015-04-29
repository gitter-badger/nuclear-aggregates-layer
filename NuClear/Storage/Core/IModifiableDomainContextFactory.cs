namespace NuClear.Storage.Core
{
    /// <summary>
    /// Интерфейс для создания domain context пригодного для модификации данных
    /// </summary>
    public interface IModifiableDomainContextFactory : IDomainContextFactory<IModifiableDomainContext> 
    {
    }
}