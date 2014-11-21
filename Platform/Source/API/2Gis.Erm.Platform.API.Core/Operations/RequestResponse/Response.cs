using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse
{
    [DataContract]
    public abstract class Response
    {
        public static readonly EmptyResponse Empty = new EmptyResponse();
    }
}
