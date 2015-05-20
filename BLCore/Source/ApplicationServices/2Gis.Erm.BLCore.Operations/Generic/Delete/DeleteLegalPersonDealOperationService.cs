using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteLegalPersonDealOperationService : IDeleteGenericEntityService<LegalPersonDeal>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDeleteLegalPersonDealService _deleteAggregateRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteLegalPersonDealOperationService(
            IDealReadModel dealReadModel, 
            IOperationScopeFactory operationScopeFactory, 
            IDeleteLegalPersonDealService deleteAggregateRepository)
        {
            _dealReadModel = dealReadModel;
            _operationScopeFactory = operationScopeFactory;
            _deleteAggregateRepository = deleteAggregateRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var link = _dealReadModel.GetLegalPersonDeal(entityId);
            var isLinkLastOne = _dealReadModel.IsLinkTheLastOneForDeal(link.Id, link.DealId);

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonDeal>())
            {
                _deleteAggregateRepository.Delete(link, isLinkLastOne);
                scope.Deleted(link).Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}