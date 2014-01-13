using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    public sealed class ReplacePriceHandler : RequestHandler<ReplacePriceRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IPriceRepository _priceRepository;
        private readonly IDeleteGenericEntityService<Price> _deletePriceService;

        public ReplacePriceHandler(ISubRequestProcessor subRequestProcessor, IPriceRepository priceRepository, IDeleteGenericEntityService<Price> deletePriceService)
        {
            _subRequestProcessor = subRequestProcessor;
            _priceRepository = priceRepository;
            _deletePriceService = deletePriceService;
        }

        protected override EmptyResponse Handle(ReplacePriceRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var targetPriceOrgUnitId = _priceRepository.GetPriceOrganizationUnitId(request.TargetPriceId);

                _deletePriceService.Delete(request.TargetPriceId);

                _subRequestProcessor.HandleSubRequest(new CopyPriceRequest
                {
                    SourcePriceId = request.SourcePriceId,
                    TargetPriceId = request.TargetPriceId,
                    OrganizationUnitId = targetPriceOrgUnitId
                }, Context);

                transaction.Complete();
                return Response.Empty;
            }
        }
    }
}