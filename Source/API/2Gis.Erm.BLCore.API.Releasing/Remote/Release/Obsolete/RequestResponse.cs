using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Obsolete
{
    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    [KnownType(typeof(StartExportRequest))]
    [KnownType(typeof(FinishExportRequest))]
    public abstract class Request
    { 
    }

    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    [KnownType(typeof(StartExportResponse))]
    [KnownType(typeof(FinishExportResponse))]
    public abstract class Response
    {
    }
}