using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IQdsComponentsFactory
    {
        IEnumerable<IQdsComponent> CreateQdsComponents();
    }
}