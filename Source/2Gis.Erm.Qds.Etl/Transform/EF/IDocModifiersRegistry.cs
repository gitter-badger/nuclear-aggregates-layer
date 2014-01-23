using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocModifiersRegistry
    {
        IDocsSelector GetModifier(Type docType);
    }
}