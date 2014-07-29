using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public interface ISubRequestProcessor
    {
        Response HandleSubRequest(Request subRequest, HandlerContext parentContext);
        Response HandleSubRequest(Request subRequest, HandlerContext parentContext, bool isParentShareTransaction);
    }
}