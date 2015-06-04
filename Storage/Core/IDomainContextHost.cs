using System;

namespace NuClear.Storage.Core
{
    /// <summary>
    /// »нтерфейс позвол€ет идентифицировать конкретный экземпл€р-хост дл€ использовани€ в рамках которого создавались domain context (хостами могут быть, например: UoW, UoWScope)
    /// </summary>
    public interface IDomainContextHost : IDisposable
    {
        Guid ScopeId { get; }
    }
}