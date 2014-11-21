using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IPersistenceServiceRuntimeFactory
    {
        object CreatePersistenceService(Type consumerType); 
    }
}