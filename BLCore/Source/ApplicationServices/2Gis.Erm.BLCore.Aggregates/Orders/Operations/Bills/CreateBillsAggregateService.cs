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
    public sealed class CreateBillsAggregateService : ICreateBillsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<Bill> _billGenericRepository; 
        private readonly IValidateBillsService _validateBillsService;

        public CreateBillsAggregateService(
            IOperationScopeFactory scopeFactory, 
            IRepository<Bill> billGenericRepository, 
            IIdentityProvider identityProvider,
            IValidateBillsService validateBillsService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _identityProvider = identityProvider;
            _validateBillsService = validateBillsService;
        }

        public void Create(Order order, IEnumerable<Bill> bills)
        {
            string report;
            if (!_validateBillsService.PreValidate(bills, out report) || !_validateBillsService.Validate(bills, order, out report))
            {
                throw new NotificationException(report);
            }

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
