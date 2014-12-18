using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bills
{
    public sealed class UpdateBillAggregateService : IUpdateBillAggregateService
    {
        private readonly IRepository<Bill> _billGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IValidateBillsService _validateBillsService;

        public UpdateBillAggregateService(IOperationScopeFactory scopeFactory, IRepository<Bill> billGenericRepository, IValidateBillsService validateBillsService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _validateBillsService = validateBillsService;
        }

        public void Update(Bill bill, IEnumerable<Bill> bills, Order order)
        {
            _validateBillsService.Validate(bills, order);
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
