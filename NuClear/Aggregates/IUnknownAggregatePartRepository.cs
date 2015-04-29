namespace NuClear.Aggregates
{
    /// <summary>
    /// ћаркерный интерфейс агрегирующих (типизированных, специфических) репозиториев частей контракта операций агрегата, дл€ которых в compile time не известен корень агрегации
    /// “.е. фактически реализовывать этот интерфейс должны open generic реализации каких-то специфических операций дл€ агрегата, при этом сам агрегат в compile time остаетс€ не известен
    /// </summary>
    public interface IUnknownAggregatePartRepository : IAggregateRepository
    {
    }
}