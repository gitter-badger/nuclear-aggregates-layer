using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.DAL.Model.Aggregates
{
    /// <summary>
    /// Абстракция для фабрики агрегирующих (типизированных, специфических) репозиториев
    /// </summary>
    public interface IAggregateRepositoryFactory
    {
        TAggregateRepository CreateRepository<TAggregateRepository>()
            where TAggregateRepository : class, IAggregateRepository;
    }
}
