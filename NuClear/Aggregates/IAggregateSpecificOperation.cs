using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Aggregates
{
    /// <summary>
    /// Маркерный интерфейс для интерфейсов представляющих конкретную бизнес операцию для элементов конкретного агрегата, с поддержкой инвариантов и т.д.
    /// Его реализуют небольшие типы (желательно SRP) инкапсулирущие изменения какой-то части агрегата, с поддержкой инвариантов.
    /// Фактически реализаторы этого интерфейса - это аналог бывших агрегирующих репозиториев, только распиленных на части.
    /// Особенности - они фактически CUD, т.е. read активность вынесена в отдельный класс сущностей - read model.
    /// Read model не может использоваться в реализациях IAggregateSpecificOperation, все что нужно для выполнения работы в реализацию  IAggregateSpecificOperation должно приходить в виде аргументов.
    /// В иерархии слоев приложения находятся на уровень ниже operation services - обеспечивая инкапсуляцию работы с DAL + поддержку инвариантов
    /// Один и тот же operation service может использовать несколько IAggregateSpecificOperation - если бизнес операция является комплексной и выполняет несколько изменений (хотя  по в DDD это и очень плохо, особенно если затрагиваются несколько агрегатов)
    /// </summary>
    /// <typeparam name="TAggregateRoot">Корень агрегата, который обрабатывается данной операцией</typeparam>
    /// <typeparam name="TOperationIdentity">Identity операции, которую выполняет тип реализующий данный интерфейс</typeparam>
    public interface IAggregateSpecificOperation<TAggregateRoot, TOperationIdentity> : IAggregatePartRepository<TAggregateRoot>
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}
