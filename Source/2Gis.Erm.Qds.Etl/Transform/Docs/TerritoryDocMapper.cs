using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class TerritoryDocMapper : IDocMapper<TerritoryDoc>
    {
        public void UpdateDocByEntity(IEnumerable<TerritoryDoc> docs, IEntityKey entity)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var territory = entity as Territory;

            if (territory == null)
                throw new NotSupportedException(entity.GetType().FullName);

            foreach (var doc in docs)
            {
                doc.Id = territory.Id;
                doc.Name = territory.Name;
            }
        }
    }
}