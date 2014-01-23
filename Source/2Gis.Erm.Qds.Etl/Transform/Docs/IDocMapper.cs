using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IDocMapper<in TDoc> where TDoc : IDoc
    {
        void UpdateDocByEntity(IEnumerable<TDoc> doc, IEntityKey entity);
    }
}