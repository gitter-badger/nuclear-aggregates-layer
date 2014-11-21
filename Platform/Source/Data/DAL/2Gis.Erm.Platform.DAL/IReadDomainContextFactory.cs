namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Интерфейс фабрики для создания domain context пригодного только для чтения
    /// </summary>
    public interface IReadDomainContextFactory
    {
        IReadDomainContext Create(DomainContextMetadata domainContextMetadata);
    }
}