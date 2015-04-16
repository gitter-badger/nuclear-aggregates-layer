using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteAdvertisementTemplateOperationService : IDeleteGenericEntityService<AdvertisementTemplate>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteAdvertisementTemplateOperationService(
            IAdvertisementRepository advertisementRepository,
            IOperationScopeFactory operationScopeFactory)
        {
            _advertisementRepository = advertisementRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, AdvertisementTemplate>())
            {
                var deleteAggregateRepository = (IDeleteAggregateRepository<AdvertisementTemplate>)_advertisementRepository;
                deleteAggregateRepository.Delete(entityId);

                operationScope.Updated<AdvertisementTemplate>(entityId);
                operationScope.Complete();

                transaction.Complete();
            }

            return null;
        }

        // not used
        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}