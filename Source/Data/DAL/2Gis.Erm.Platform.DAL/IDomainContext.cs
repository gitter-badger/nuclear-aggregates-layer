using System;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Маркерный интерфейс для domaincontext - абстракция хранилища, с которым работают сущностные репозитории
    /// </summary>
    public interface IDomainContext : IDisposable
    {
    }
}