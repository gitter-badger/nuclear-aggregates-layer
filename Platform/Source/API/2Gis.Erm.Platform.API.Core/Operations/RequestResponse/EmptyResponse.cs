using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse
{
    [DataContract]
    public sealed class EmptyResponse : Response
    {
        // please use Response.Empty
        internal EmptyResponse()
        {
        }
    }
}