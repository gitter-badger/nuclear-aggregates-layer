using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bargains;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bargains
{
    public class CloseClientBargainsAggregateService : ICloseClientBargainsAggregateService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureRepository<Bargain> _bargainGenericRepository;


        public CloseClientBargainsAggregateService(IOperationScopeFactory scopeFactory, ISecureRepository<Bargain> bargainGenericRepository)
        {
            _scopeFactory = scopeFactory;
            _bargainGenericRepository = bargainGenericRepository;
        }

        public void CloseBargains(IEnumerable<Bargain> bargains, DateTime closeDate)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<BulkCloseClientBargainsIdentity>())
            {
                foreach (var bargain in bargains)
                {
                    bargain.ClosedOn = closeDate;
                    _bargainGenericRepository.Update(bargain);
                    scope.Updated<Bargain>(bargain.Id);
                }

                _bargainGenericRepository.Save();
                scope.Complete();
            }
        }
    }
}
