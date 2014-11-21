using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.DAL.Model.Aggregates
{
    /// <summary>
    /// Абстракция для фабрики агрегирующих (типизированных, специфических) репозиториев, 
    /// с учетом для какого host domain context создается репозиторий (конкретный UoWScope, либо сам UoW)
    /// </summary>
    public interface IAggregateRepositoryForHostFactory
    {
        TAggregateRepository CreateRepository<TAggregateRepository>(IDomainContextHost domainContextHost) where TAggregateRepository : class, IAggregateRepository;
    }
}
