using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteBargainService : IDeleteGenericEntityService<Bargain>
    {
        private readonly IBargainRepository _bargainRepository;

        public DeleteBargainService(IBargainRepository bargainRepository)
        {
            _bargainRepository = bargainRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var bargainInfo = _bargainRepository.GetBargainUsage(entityId);
            if (bargainInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }
            if (bargainInfo.OrderNumbers.Any())
            {
                throw new ArgumentException(CreateExeptionMessage(bargainInfo.OrderNumbers));
            }

            _bargainRepository.Delete(bargainInfo.Bargain);

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var bargainInfo = _bargainRepository.GetBargainUsage(entityId);

            if (bargainInfo == null)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };

            if (bargainInfo.OrderNumbers.Any())
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = CreateExeptionMessage(bargainInfo.OrderNumbers)
                };

            return new DeleteConfirmationInfo
            {
                EntityCode = bargainInfo.Bargain.Number,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }

        private static string CreateExeptionMessage(IEnumerable<string> orders)
        {
            return string.Format(BLResources.CannotRemoveBargainBecauseItIsLinkedWithOrders, string.Join(", ", orders));
        }
    }
}