using System;

namespace NuClear.Storage.Core
{
    /// <summary>
    /// Маркерный интерфейс для domaincontext - абстракция хранилища, с которым работают сущностные репозитории
    /// </summary>
    public interface IDomainContext : IDisposable
    {
    }
}