﻿using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateOrganizationUnitService : IActivateGenericEntityService<OrganizationUnit>
    {
        private readonly IUserRepository _userRepository;

        public ActivateOrganizationUnitService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int Activate(long entityId)
        {
            int result = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = _userRepository as IActivateAggregateRepository<OrganizationUnit>;
                result = activateAggregateRepository.Activate(entityId);

                transaction.Complete();
            }

            return result;
        }
    }
}
