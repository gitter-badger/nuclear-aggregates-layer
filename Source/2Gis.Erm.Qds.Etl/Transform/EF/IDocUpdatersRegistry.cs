using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocUpdatersRegistry
    {
        IDocsUpdater GetUpdater(Type docType);
        void AddUpdater(IDocsUpdater updater);
    }
}