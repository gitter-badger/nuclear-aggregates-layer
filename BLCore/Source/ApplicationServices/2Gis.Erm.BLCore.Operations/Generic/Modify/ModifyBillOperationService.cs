using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyBillOperationService : IModifyBusinessModelEntityService<Bill>
    {
        private readonly IUpdateBillAggregateService _updateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBusinessModelEntityObtainer<Bill> _billObtainer;
        private readonly IOrderReadModel _orderReadModel;

        public ModifyBillOperationService(IUpdateBillAggregateService updateService,
                                          IOperationScopeFactory operationScopeFactory,
                                          IBusinessModelEntityObtainer<Bill> billObtainer,
                                          IOrderReadModel orderReadModel)
        {
            _updateService = updateService;
            _operationScopeFactory = operationScopeFactory;
            _billObtainer = billObtainer;
            _orderReadModel = orderReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var bill = _billObtainer.ObtainBusinessModelEntity(domainEntityDto);
            var order = _orderReadModel.GetOrderSecure(bill.OrderId);
            var otherBills = _orderReadModel.GetBillsForOrder(bill.OrderId).Where(x => x.Id != bill.Id);
            var orderBills = otherBills.Concat(new[] { bill }).ToArray();

            if (bill.IsNew())
            {
                throw new OperationException<Bill, CreateIdentity>(BLResources.OperationIsNotSpecified);
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Bill>())
            {
                _updateService.Update(bill, orderBills, order);
                scope.Updated(bill)
                     .Updated(order)
                     .Complete();
            }

            return bill.Id;
        }
    }
}