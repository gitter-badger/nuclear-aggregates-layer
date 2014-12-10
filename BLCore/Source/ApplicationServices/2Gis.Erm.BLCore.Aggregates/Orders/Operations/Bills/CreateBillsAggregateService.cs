using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IEvaluateBillNumberService _evaluateBillNumberService;

        public CreateBillsAggregateService(
            IOperationScopeFactory scopeFactory, 
            IRepository<Bill> billGenericRepository, 
            IIdentityProvider identityProvider,
            IValidateBillsService validateBillsService, 
            IEvaluateBillNumberService evaluateBillNumberService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _identityProvider = identityProvider;
            _validateBillsService = validateBillsService;
            _evaluateBillNumberService = evaluateBillNumberService;
        }

        public void Create(Order order, IEnumerable<Bill> bills)
        {
            var billsArray = bills as Bill[] ?? bills.ToArray();

            var numberEvaluation = billsArray.Length == 1
                ? new Func<Bill, int, string>((bill, index) => _evaluateBillNumberService.Evaluate(bill.BillNumber, order.Number))
                : new Func<Bill, int, string>((bill, index) => _evaluateBillNumberService.Evaluate(bill.BillNumber, order.Number, index + 1));

            for (var i = 0; i < billsArray.Length; i++)
            {
                billsArray[i].BillNumber = numberEvaluation.Invoke(billsArray[i], i);
            }

            string report;
            if (!_validateBillsService.PreValidate(billsArray, out report) || !_validateBillsService.Validate(billsArray, order, out report))
            {
                throw new NotificationException(report);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Bill>())
            {
                foreach (var bill in billsArray)
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
