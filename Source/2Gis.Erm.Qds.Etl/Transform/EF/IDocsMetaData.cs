using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocsMetaData
    {
        IDocsUpdater[] GetDocsUpdaters(Type partType);
    }
}