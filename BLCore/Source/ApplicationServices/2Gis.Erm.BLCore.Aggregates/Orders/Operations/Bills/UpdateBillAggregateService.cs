using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
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

        public UpdateBillAggregateService(IOperationScopeFactory scopeFactory, IRepository<Bill> billGenericRepository)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
        }

        public void Update(Bill bill)
        {
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
