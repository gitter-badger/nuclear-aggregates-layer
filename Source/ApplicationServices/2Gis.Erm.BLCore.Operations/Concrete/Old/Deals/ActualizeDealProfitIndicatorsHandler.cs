using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals
{
    public sealed class ActualizeDealProfitIndicatorsHandler : RequestHandler<ActualizeDealProfitIndicatorsRequest, EmptyResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealActualizeDealProfitIndicatorsAggregateService _dealActualizeDealProfitIndicatorsAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActualizeDealProfitIndicatorsHandler(
            IDealReadModel dealReadModel,
            IDealActualizeDealProfitIndicatorsAggregateService dealActualizeDealProfitIndicatorsAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _dealReadModel = dealReadModel;
            _dealActualizeDealProfitIndicatorsAggregateService = dealActualizeDealProfitIndicatorsAggregateService;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(ActualizeDealProfitIndicatorsRequest request)
        {
            // TODO {all, 22.08.2013}: вероятно, тут должна быть более специфичная операция
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Deal>())
            {
                // TODO {all, 04.11.2013}: при рефакторинге ApplicationServices для обеспечения SRP - проверить есть ли реальная необходимость в отдельном режиме без обработки deal.ActualProfit - если нет выпилить флаг из сигнатуры
                var dealInfos = _dealReadModel.GetInfoForActualizeProfits(request.DealIds, true);
                _dealActualizeDealProfitIndicatorsAggregateService.Actualize(dealInfos);

                scope.Complete();
            }

            return Response.Empty;
        }
    }
}