using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL.Model.Aggregates;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// контейнер DomainContext’ов, контекст бизнес операции. 1 экземпляр на бизнес-операцию, временем жизни управляет DI
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAggregateRepositoryFactory
    {
        /// <summary>
        /// Возвращает для указанного host все связанные с ним domain context, допускающие модификацию данных
        /// </summary>
        IEnumerable<IModifiableDomainContext> GetModifiableDomainContexts(IDomainContextHost host);

        /// <summary>
        /// Забирает из под контроля UoW все domain contexts, связанные с указанным host. 
        /// и возвращает их в качестве результата.
        /// Ответственность за вызов dispose для полученных через результат domain contexts лежит на вызывающем коде
        /// </summary>
        IEnumerable<IModifiableDomainContext> DeattachModifiableDomainContexts(IDomainContextHost host);

        /// <summary>
        /// Создает новый UoWScope
        /// </summary>
        IUnitOfWorkScope CreateScope();
    }
}