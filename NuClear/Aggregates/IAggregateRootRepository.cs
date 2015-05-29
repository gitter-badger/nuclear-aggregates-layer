using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// ћаркерный интерфейс дл€ агрегирующих (типизированных, специфических) репозиториев конкретных aggregate root
    /// позвол€ющий указать aggregate root дл€ агрегата. 
    /// “.е. расшир€ть этот интерфейс должны только главные интерфейсы агрегирующего репозитори€ (чаще всего такой один), 
    /// которые св€зывают контракт операций дл€ агрегата с самим агрегатом, через aggregate root
    /// </summary>
    public interface IAggregateRootService<TAggregateRoot> : IAggregateService
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}