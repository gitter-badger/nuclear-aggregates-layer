﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bills
{
    public sealed class BulkDeleteBillAggregateService : IBulkDeleteBillAggregateService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<Bill> _billGenericRepository;
        private readonly IValidateBillsService _validateBillsService;

        public BulkDeleteBillAggregateService(IOperationScopeFactory scopeFactory, IRepository<Bill> billGenericRepository, IValidateBillsService validateBillsService)
        {
            _scopeFactory = scopeFactory;
            _billGenericRepository = billGenericRepository;
            _validateBillsService = validateBillsService;
        }

        public void DeleteBills(Order order, IEnumerable<Bill> bills)
        {
            string report;
            if (!_validateBillsService.Validate(new Bill[0], order, out report))
            {
                throw new NotificationException(report);
            }

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