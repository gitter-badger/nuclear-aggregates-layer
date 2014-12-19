using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bills
{
    public sealed class BulkCreateBillAggregateService : IBulkCreateBillAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<Bill> _billGenericRepository; 
        private readonly IBillsConsistencyService _billsConsistencyService;

        public BulkCreateBillAggregateService(
            IOperationScopeFactory scopeFactory, 
            IRepository<Bill> billGenericRepository, 
            IIdentityProvider identityProvider,
            IBillsConsistencyService billsConsistencyService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _identityProvider = identityProvider;
            _billsConsistencyService = billsConsistencyService;
        }

        public void Create(Order order, IEnumerable<Bill> bills)
        {
            _billsConsistencyService.Validate(bills, order);
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Bill>())
            {
                foreach (var bill in bills)
                {
                    _identityProvider.SetFor(bill);
                    _billGenericRepository.Add(bill);
                    scope.Added(bill);
                }

                _billGenericRepository.Save();
                scope.Complete();
            }
        }
    }
}
