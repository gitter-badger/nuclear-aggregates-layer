using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public interface IRequestProcessor
    {
        Response HandleRequest(Request request);
    }
}