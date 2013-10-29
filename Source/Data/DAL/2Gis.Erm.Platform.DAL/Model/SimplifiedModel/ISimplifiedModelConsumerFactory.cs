using System;

namespace DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel
{
    public interface ISimplifiedModelConsumerRuntimeFactory
    {
        object CreateConsumer(Type consumerType);
    }
}
