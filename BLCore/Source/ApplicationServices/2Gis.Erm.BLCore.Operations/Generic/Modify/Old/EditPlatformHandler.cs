using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPlatformHandler : RequestHandler<EditRequest<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>, EmptyResponse>
    {
        private readonly IPlatformService _platformService;

        public EditPlatformHandler(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        protected override EmptyResponse Handle(EditRequest<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform> request)
        {
            var platform = request.Entity;
            _platformService.CreateOrUpdate(platform);
            return Response.Empty;
        }
    }
}