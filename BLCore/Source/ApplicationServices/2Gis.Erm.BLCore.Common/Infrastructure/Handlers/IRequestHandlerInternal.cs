namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    internal interface IRequestHandlerInternal : IRequestHandler
    {
        HandlerContext Context { set; }
    }
}