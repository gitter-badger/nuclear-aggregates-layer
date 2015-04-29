using System;

using NuClear.Aggregates.Storage;
using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// область внутри бизнес операции, контекст участка бизнес-операции. Время жизни меньше, чем у UnitOfWork. Может быть несколько экзепляров UnitOfWorkScope для одного UnitOfWork, как независимых, так и вложенных. 
    /// После окончания жизни UnitOfWorkScope удаляются DomainContext’ы, созданные внутри него.
    /// </summary>
    public interface IUnitOfWorkScope : IDomainContextHost, IAggregateRepositoryFactory, IDisposable
    {
        void Complete();
    }
}