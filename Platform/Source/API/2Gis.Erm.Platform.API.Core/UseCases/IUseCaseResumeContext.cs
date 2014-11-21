using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.Platform.API.Core.UseCases
{
    public interface IUseCaseResumeContext<TRequest>
        where TRequest : Request
    {
        Response UseCaseResume(Request subRequest);
        Response UseCaseResume(Request subRequest, bool isParentShareTransaction);
        TRequest Request { get; }
    }
}
