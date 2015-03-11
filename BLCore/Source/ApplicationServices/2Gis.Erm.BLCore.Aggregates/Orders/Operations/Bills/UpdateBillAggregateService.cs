using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bills
{
    public sealed class UpdateBillAggregateService : IUpdateBillAggregateService
    {
        private readonly ISecureRepository<Bill> _billGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBillsConsistencyService _billsConsistencyService;

        public UpdateBillAggregateService(IOperationScopeFactory scopeFactory, ISecureRepository<Bill> billGenericRepository, IBillsConsistencyService billsConsistencyService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _billsConsistencyService = billsConsistencyService;
        }

        public void Update(Bill bill, IEnumerable<Bill> bills, Order order)
        {
            _billsConsistencyService.Validate(bills, order);
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Bill>())
            {
                _billGenericRepository.Update(bill);
                _billGenericRepository.Save();
                scope.Updated(bill)
                     .Complete();
            }
        }
    }
}
