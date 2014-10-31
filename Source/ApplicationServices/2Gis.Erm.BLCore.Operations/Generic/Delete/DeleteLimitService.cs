using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public class DeleteLimitService : IDeleteGenericEntityService<Limit>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IDeleteLimitAggregateService _deleteLimitAggregateService;

        public DeleteLimitService(IAccountReadModel accountReadModel, IDeleteLimitAggregateService deleteLimitAggregateService)
        {
            _accountReadModel = accountReadModel;
            _deleteLimitAggregateService = deleteLimitAggregateService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var limit = _accountReadModel.GetLimitById(entityId);
            if (limit == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            if (!limit.IsActive)
            {
                throw new ArgumentException(BLResources.CantDeleteInactiveLimit);
            }

            _deleteLimitAggregateService.Delete(limit);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var limit = _accountReadModel.GetLimitById(entityId);
            if (limit == null)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };
            }

            if (!limit.IsActive)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.CantDeleteInactiveLimit
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