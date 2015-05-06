using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Olap
{
    public sealed class ImportFirmPromisingHandler : RequestHandler<ImportFirmPromisingRequest, EmptyResponse>
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IUserContext _userContext;

        public ImportFirmPromisingHandler(IFirmRepository firmRepository, IUserContext userContext)
        {
            _firmRepository = firmRepository;
            _userContext = userContext;
        }

        protected override EmptyResponse Handle(ImportFirmPromisingRequest request)
        {
            _firmRepository.ImportFirmPromisingValues(_userContext.Identity.Code);
            return Response.Empty;
        }
    }
}
