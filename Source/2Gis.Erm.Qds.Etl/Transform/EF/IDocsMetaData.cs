using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocsMetaData
    {
        IEnumerable<IDocsUpdater> GetDocsUpdaters(Type partType);
    }
}