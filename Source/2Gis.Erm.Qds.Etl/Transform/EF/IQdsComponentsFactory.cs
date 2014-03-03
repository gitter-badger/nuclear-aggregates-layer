using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IQdsComponentsFactory
    {
        IEnumerable<IQdsComponent> CreateQdsComponents();
    }
}