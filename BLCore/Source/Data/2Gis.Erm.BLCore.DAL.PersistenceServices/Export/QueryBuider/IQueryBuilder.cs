﻿using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider
{
    // TODO {a.rechkalov, 16.08.2013}: Нужно будет объединить эту абстракцию и IExportRepository с абстракцией Read Model, т.к. у них одна и та же цель
    public interface IQueryBuilder<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        IQueryable<TEntity> Create(params IFindSpecification<TEntity>[] filterSpecifications);
    }
}
