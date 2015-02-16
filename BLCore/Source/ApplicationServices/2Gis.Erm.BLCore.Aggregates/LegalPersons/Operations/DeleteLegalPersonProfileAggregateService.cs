using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonProfileAggregateService : IDeleteLegalPersonProfileAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly IRepository<Order> _orderRepository;

        public DeleteLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,            
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository, 
            IRepository<Order> orderRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _orderRepository = orderRepository;
        }

        public void Delete(LegalPersonProfile legalPersonProfile, IEnumerable<Order> referringOrders)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
            {
                foreach (var order in referringOrders)
                {
                    order.LegalPersonProfileId = null;
                    _orderRepository.Update(order);
                    operationScope.Updated(order);
                }

                _legalPersonProfileSecureRepository.Delete(legalPersonProfile);
                operationScope.Deleted(legalPersonProfile);
                _legalPersonProfileSecureRepository.Save();
                _orderRepository.Save();

                operationScope.Complete();
            }
        }
    }
}