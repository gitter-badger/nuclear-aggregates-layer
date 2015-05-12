using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class UnpublishPriceHandler : RequestHandler<UnpublishPriceRequest, EmptyResponse>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IPriceRepository _priceRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UnpublishPriceHandler(IPriceReadModel priceReadModel, IPriceRepository priceRepository, IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _priceRepository = priceRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(UnpublishPriceRequest request)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<UnpublishPriceIdentity>())
            {
                var price = _priceReadModel.GetPrice(request.PriceId);
                if (price == null)
                {
                    throw new EntityNotFoundException(typeof(Price), request.PriceId);
                }

                ValidatePrice(price.Id, price.OrganizationUnitId);
                
                _priceRepository.Unpublish(price);

                operationScope.Complete();

            return Response.Empty;
        }
    }

        private void ValidatePrice(long priceId, long organizationUnitId)
        {
            // TODO {v.lapeev, 07.11.2013}: Код ниже выполняет проверки актуальности прайса и наличия размещаемых заказов. Нужно включить эти проверки снова, как только процесс работы с прайсами наладится
            /*
            var actualPriceId = _priceReadModel.GetActualPriceId(organizationUnitId);
            if (priceId == actualPriceId)
            {
                throw new NotificationException(BLResources.CannotUnpublishActualPrice);
            }

            var isLinked = _priceReadModel.IsPriceLinked(priceId);
            if (isLinked)
            {
                throw new NotificationException(BLResources.CannotUnpublishLinkedPrice);
            }
            */
        }
    }
}