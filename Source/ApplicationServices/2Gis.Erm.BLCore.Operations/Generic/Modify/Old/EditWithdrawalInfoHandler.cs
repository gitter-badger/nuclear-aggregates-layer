using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditWithdrawalInfoHandler : RequestHandler<EditRequest<WithdrawalInfo>, EmptyResponse>
    {
        private readonly IWithdrawalInfoRepository _withdrawalInfoRepository;

        public EditWithdrawalInfoHandler(IWithdrawalInfoRepository withdrawalInfoRepository)
        {
            _withdrawalInfoRepository = withdrawalInfoRepository;
        }

        protected override EmptyResponse Handle(EditRequest<WithdrawalInfo> request)
        {
            _withdrawalInfoRepository.CreateOrUpdate(request.Entity);   
            return Response.Empty;
        }
    }
}