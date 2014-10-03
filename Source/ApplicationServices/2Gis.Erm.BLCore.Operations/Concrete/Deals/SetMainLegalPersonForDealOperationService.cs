using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Deals
{
    public sealed class SetMainLegalPersonForDealOperationService : ISetMainLegalPersonForDealOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUpdateLegalPersonDealAggregateService _updateLegalPersonDealAggregateService;

        public SetMainLegalPersonForDealOperationService(IDealReadModel dealReadModel,
                                                         IOperationScopeFactory scopeFactory,
                                                         IUpdateLegalPersonDealAggregateService updateLegalPersonDealAggregateService)
        {
            _dealReadModel = dealReadModel;
            _scopeFactory = scopeFactory;
            _updateLegalPersonDealAggregateService = updateLegalPersonDealAggregateService;
        }

        public void SetMainLegalPerson(long dealId, long legalPersonId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<SetMainLegalPersonForDealIdentity>())
            {
                var newMainLegalPersonDeal = _dealReadModel.GetLegalPersonDeal(dealId, legalPersonId);

                if (newMainLegalPersonDeal == null)
                {
                    throw new EntityNotFoundException(typeof(LegalPersonDeal));
                }

                var currentMainLegalPerson = _dealReadModel.GetMainLegalPersonForDeal(newMainLegalPersonDeal.DealId);
                if (currentMainLegalPerson != null)
                {
                    currentMainLegalPerson.IsMain = false;
                    _updateLegalPersonDealAggregateService.Update(currentMainLegalPerson);
                    scope.Updated(currentMainLegalPerson);
                }

                newMainLegalPersonDeal.IsMain = true;
                _updateLegalPersonDealAggregateService.Update(newMainLegalPersonDeal);
                scope.Updated(newMainLegalPersonDeal);

                scope.Complete();
            }
        }
    }
}