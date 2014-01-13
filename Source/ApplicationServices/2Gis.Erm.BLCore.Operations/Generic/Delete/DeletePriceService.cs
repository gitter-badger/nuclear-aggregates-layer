using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePriceService : IDeleteGenericEntityService<Price>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                using (var scope = _unitOfWork.CreateScope())
                {
                    var priceRepository = scope.CreateRepository<IPriceRepository>();

                    var pricePublishedForToday = priceRepository.PricePublishedForToday(entityId);
                    if (pricePublishedForToday)
                    {
                        throw new ArgumentException(BLResources.PriceInActionCannotBeDeactivated);
                    }

                    var priceHasLinkedOrders = priceRepository.PriceHasLinkedOrders(entityId);
                    if (priceHasLinkedOrders)
                    {
                        throw new ArgumentException(BLResources.PriceIsLinkedWithActiveOrdersAndCannotBeDeleted);
                    }
                    priceRepository.DeleteWithSubentities(entityId);

                    scope.Complete();
                }
                transaction.Complete();
                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var priceRepository = _unitOfWork.CreateRepository<PriceRepository>();

            var pricePublishedForToday = priceRepository.PricePublishedForToday(entityId);
            if (pricePublishedForToday)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.PriceInActionCannotBeDeactivated
                };
            }

            var priceExists = priceRepository.PriceExists(entityId);
            if (!priceExists)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };
            }

            return new DeleteConfirmationInfo
            {
                EntityCode = string.Empty,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}