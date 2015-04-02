using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class ReplacePriceOperationService : IReplacePriceOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICopyPriceOperationService _copyPriceOperationService;
        private readonly IDeleteGenericEntityService<Price> _deletePriceService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ReplacePriceOperationService(IPriceReadModel priceReadModel,
                                            ICopyPriceOperationService copyPriceOperationService,
                                            IDeleteGenericEntityService<Price> deletePriceService,
                                            IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _copyPriceOperationService = copyPriceOperationService;
            _deletePriceService = deletePriceService;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Replace(long sourcePriceId, long targetPriceId)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ReplacePriceIdentity>())
            {
                _deletePriceService.Delete(targetPriceId);

                var targetPriceDto = _priceReadModel.GetPriceDto(targetPriceId);
                _copyPriceOperationService.Copy(sourcePriceId,
                                                targetPriceDto.OrganizationUnitId,
                                                targetPriceDto.CreateDate,
                                                targetPriceDto.PublishDate,
                                                targetPriceDto.BeginDate);

                operationScope.Complete();
            }
        }
    }
}