using System;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// »нтерфейс позвол€ет идентифицировать конкретный экземпл€р-хост дл€ использовани€ в рамках которого создавались domain context (хостами могут быть, например: UoW, UoWScope)
    /// </summary>
    public interface IDomainContextHost
    {
        Guid ScopeId { get; }
    }
}