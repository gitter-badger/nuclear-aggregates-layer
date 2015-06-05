namespace NuClear.Storage.Core
{
    /// <summary>
    /// Интерфейс фабрики для создания domain context пригодного только для чтения
    /// </summary>
    public interface IReadableDomainContextFactory
    {
        IReadableDomainContext Create(DomainContextMetadata domainContextMetadata);
    }
}