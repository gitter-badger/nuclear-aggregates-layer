using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocsMetaData
    {
        IDocsSelector[] GetDocsSelectors(Type partType);
    }
}