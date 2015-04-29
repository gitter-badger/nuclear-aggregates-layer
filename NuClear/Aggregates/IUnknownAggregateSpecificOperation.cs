using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Aggregates
{
    /// <summary>
    /// Версия для использования в open generic реализациях операций уровня агрегатов (для них не известен в compile time тип сущности, 
    /// которая будет обрабатываться операцией, т.е. тип которым закроют generic, а => не известен и корень агрегации для этой сущности).
    /// Полное описание см. у полной версии интерфейса IAggregateSpecificOperation.
    /// </summary>
    /// <typeparam name="TOperationIdentity">Identity операции, которую выполняет тип реализующий данный интерфейс</typeparam>
    public interface IUnknownAggregateSpecificOperation<TOperationIdentity> : IUnknownAggregatePartRepository
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
    {
    }
}