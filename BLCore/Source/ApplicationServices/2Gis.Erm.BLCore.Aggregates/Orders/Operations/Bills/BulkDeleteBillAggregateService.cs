using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bills
{
    public sealed class BulkDeleteBillAggregateService : IBulkDeleteBillAggregateService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<Bill> _billGenericRepository;
        private readonly IBillsConsistencyService _billsConsistencyService;

        public BulkDeleteBillAggregateService(IOperationScopeFactory scopeFactory, IRepository<Bill> billGenericRepository, IBillsConsistencyService billsConsistencyService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _billsConsistencyService = billsConsistencyService;
        }

        public void DeleteBills(Order order, IEnumerable<Bill> bills)
        {
            _billsConsistencyService.Validate(new Bill[0], order);
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeleteIdentity, Bill>())
            {
                foreach (var bill in bills)
                {
                    _billGenericRepository.Delete(bill);
                    scope.Deleted(bill);
                }
                
                _billGenericRepository.Save();
                scope.Complete();
            }
        }
    }
}