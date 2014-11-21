using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public interface IRequestHandler
    {
        Response Handle(Request request);
    }
}