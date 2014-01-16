using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Obsolete
{
    [ServiceContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public interface IErmService
    {
        [OperationContract]
        Response Handle(Request request);
    }
}