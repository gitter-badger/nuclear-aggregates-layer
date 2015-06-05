namespace NuClear.Storage.Core
{
    /// <summary>
    /// Позволяет получить доступ к IPersistenceChangesRegistry для слоя persistence service (в том числе и IRepository)
    /// Зачем нужен ещё и provider - т.к. цель существования PersistenceChangesRegistry - отражать изменения вносимые в рамках DAL при выполнении конкретных usecase, 
    /// то соответсвенно в DAL нужно получать доступ к реализациям IPersistenceChangesRegistry специально выделенной для данного usecase.
    /// Однако, в рамках perrequest (perwebrequest, peroperationcontext и т.п.) поведения нет гарантий выполнения только одного usecase в рамках запроса (возможно, пока нет), 
    /// хотя это и не хорошо, никто не мешает запустить несколько операций последовательно и т.п.
    /// Т.о. наличие provider позволяет не регистрировать IPersistenceChangesRegistry в DI контейнере perrequest и т.д., а получать его экземпляр из processingcontext (здесь предполагается, что processing context корректно чистится от деталей usecase после его окончания)
    /// Небольшой бонус - если usecase не поместил в контекст экземпляр IPersistenceChangesRegistry, то возможно он не хочет отслеживать изменения (например, он не обернут в operationscope) =>
    /// у provider есть возможность уже по ситуации принять решение, и подсунуть, NULL-паттерн реализацию IPersistenceChangesRegistry
    /// </summary>
    public interface IPersistenceChangesRegistryProvider
    {
        IPersistenceChangesRegistry ChangesRegistry { get; }
    }
}