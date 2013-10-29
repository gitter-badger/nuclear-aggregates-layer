using System;

namespace DoubleGis.Erm.Platform.DAL.Model.Aggregates
{
    public interface IAggregatesLayerRuntimeFactory
    {
        object CreateRepository(Type aggregateRepositoryType);
        object CreateReadModel(Type aggregateReadModelType);
    }
}
