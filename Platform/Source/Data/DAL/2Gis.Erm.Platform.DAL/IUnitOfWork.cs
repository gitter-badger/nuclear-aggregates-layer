using System;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// контейнер DomainContext’ов, контекст бизнес операции. 1 экземпляр на бизнес-операцию, временем жизни управляет DI
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Создает новый UoWScope
        /// </summary>
        IUnitOfWorkScope CreateScope();
    }
}