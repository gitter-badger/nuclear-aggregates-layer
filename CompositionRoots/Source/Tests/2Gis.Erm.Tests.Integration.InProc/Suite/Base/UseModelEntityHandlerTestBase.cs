using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public abstract class UseModelEntityHandlerTestBase<TEntity, TRequest, TResponse> : UseModelEntityTestBase<TEntity>
        where TEntity : class, IEntity
        where TRequest : Request
        where TResponse : Response
    {
        private readonly IPublicService _publicService;

        protected UseModelEntityHandlerTestBase(IPublicService publicService,
                                                IAppropriateEntityProvider<TEntity> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _publicService = publicService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(TEntity modelEntity)
        {
            TRequest request;
            if (!TryCreateRequest(modelEntity, out request))
            {
                return OrdinaryTestResult.As.Ignored.WithReport("Can't create request");
            }

            var response = _publicService.Handle(request);

            var typedResponse = response as TResponse;
            if (typedResponse == null)
            {
                return OrdinaryTestResult.As.Failed.WithReport("Can't cast response of type {0} to type {1}", response.GetType().Name, typeof(TResponse).Name);
            }

            return ResponseAsserter.Assert(typedResponse);
        }

        protected virtual IResponseAsserter<TResponse> ResponseAsserter
        {
            get { return new EmptyResponseAsserter(); }
        }

        protected abstract bool TryCreateRequest(TEntity modelEntity, out TRequest request);
    }
}