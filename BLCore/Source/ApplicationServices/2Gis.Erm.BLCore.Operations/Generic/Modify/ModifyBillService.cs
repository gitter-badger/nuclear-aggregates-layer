using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyBillService : IModifyBusinessModelEntityService<Bill>
    {
        private readonly IUpdateBillAggregateService _updateService;
        private readonly IValidateBillsService _validateBillsService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBusinessModelEntityObtainer<Bill> _billObtainer;
        private readonly IOrderReadModel _orderReadModel;

        public ModifyBillService(IUpdateBillAggregateService updateService,
                                 IValidateBillsService validateBillsService,
                                 IOperationScopeFactory operationScopeFactory,
                                 IBusinessModelEntityObtainer<Bill> billObtainer,
                                 IOrderReadModel orderReadModel)
        {
            _updateService = updateService;
            _validateBillsService = validateBillsService;
            _operationScopeFactory = operationScopeFactory;
            _billObtainer = billObtainer;
            _orderReadModel = orderReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var bill = _billObtainer.ObtainBusinessModelEntity(domainEntityDto);
            var order = _orderReadModel.GetOrderSecure(bill.OrderId);

            string report;
            if (!_validateBillsService.PreValidate(new[] { bill }, out report) || !_validateBillsService.Validate(new[] { bill }, order, out report))
            {
                throw new NotificationException(report);
            }

            if (bill.IsNew())
            {
                throw new OperationException<Bill, CreateIdentity>("Операция создания не определена");
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Bill>())
            {
                _updateService.Update(bill);
                scope.Updated(bill)
                     .Complete();
            }

            return bill.Id;
        }
    }
}